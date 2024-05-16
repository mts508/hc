using AbilityContextNamespace;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelect_ChargeSingleStep : GenericAbility_TargetSelectBase
{
	[Separator("Targeting Properties", true)]
	public AbilityAreaShape m_destShape;

	[Separator("Sequences", true)]
	public GameObject m_castSequencePrefab;

	private TargetSelectMod_ChargeSingleStep m_targetSelMod;

	public override string GetUsageForEditor()
	{
		return "Intended for single click charge abilities. Can add shape field to hit targets on destination.";
	}

	public override List<AbilityUtil_Targeter> CreateTargeters(Ability ability)
	{
		AbilityUtil_Targeter_Charge item = new AbilityUtil_Targeter_Charge(ability, GetDestShape(), IgnoreLos(), AbilityUtil_Targeter_Shape.DamageOriginType.CenterOfShape, IncludeEnemies(), IncludeAllies());
		List<AbilityUtil_Targeter> list = new List<AbilityUtil_Targeter>();
		list.Add(item);
		return list;
	}

	protected override void OnTargetSelModApplied(TargetSelectModBase modBase)
	{
		m_targetSelMod = (modBase as TargetSelectMod_ChargeSingleStep);
	}

	protected override void OnTargetSelModRemoved()
	{
		m_targetSelMod = null;
	}

	public AbilityAreaShape GetDestShape()
	{
		AbilityAreaShape result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_destShapeMod.GetModifiedValue(m_destShape);
		}
		else
		{
			result = m_destShape;
		}
		return result;
	}

	public override bool HandleCustomTargetValidation(Ability ability, ActorData caster, AbilityTarget target, int targetIndex, List<AbilityTarget> currentTargets)
	{
		BoardSquare boardSquareSafe = Board.Get().GetSquare(target.GridPos);
		if (boardSquareSafe != null && boardSquareSafe.IsValidForGameplay())
		{
			if (boardSquareSafe != caster.GetCurrentBoardSquare())
			{
				while (true)
				{
					switch (5)
					{
					case 0:
						break;
					default:
					{
						int numSquaresInPath;
						return KnockbackUtils.CanBuildStraightLineChargePath(caster, boardSquareSafe, caster.GetCurrentBoardSquare(), false, out numSquaresInPath);
					}
					}
				}
			}
		}
		return false;
	}

	public override ActorData.MovementType GetMovementType()
	{
		return ActorData.MovementType.Charge;
	}

    /*

    public override void CalcHitTargets(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        this.ResetContextData();
        base.CalcHitTargets(targets, caster, nonActorTargetInfo);
        if (this.m_bounceHitTargeting)
        {
            Vector3 loSCheckPos = caster.GetLoSCheckPos(caster.GetSquareAtPhaseStart());
            Vector3 hitOrigin;
            List<ActorData> bounceHitActors = this.GetBounceHitActors(targets, loSCheckPos, caster, out hitOrigin, nonActorTargetInfo);
            List<ActorData> list = null;
            if (bounceHitActors.Count > 0)
            {
                list = this.GetHitActorsInShape(targets, caster, nonActorTargetInfo);
                using (List<ActorData>.Enumerator enumerator = bounceHitActors.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ActorData item = enumerator.Current;
                        list.Remove(item);
                    }
                    goto IL_84;
                }
            }
            list = new List<ActorData>();
        IL_84:
            int numSquaresInProcessedEvade = ServerActionBuffer.Get().GetNumSquaresInProcessedEvade(caster);
            int value = Mathf.Max(0, numSquaresInProcessedEvade - 1);
            this.GetNonActorSpecificContext().SetValue(ContextKeys.s_directHitSquareCount.GetKey(), value);
            foreach (ActorData actor in bounceHitActors)
            {
                this.AddHitActor(actor, loSCheckPos, false);
                base.SetActorContext(actor, TargetSelect_ChargeSingleStep.s_isDirectHit.GetKey(), 1);
            }
            using (List<ActorData>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ActorData actor2 = enumerator.Current;
                    this.AddHitActor(actor2, hitOrigin, false);
                    base.SetActorContext(actor2, TargetSelect_ChargeSingleStep.s_isDirectHit.GetKey(), 0);
                }
                return;
            }
        }
        Vector3 hitOrigin2 = Board.Get().GetSquare(targets[0].GridPos).ToVector3();
        foreach (ActorData actor3 in this.GetHitActorsInShape(targets, caster, nonActorTargetInfo))
        {
            this.AddHitActor(actor3, hitOrigin2, false);
        }
    }

    private List<ActorData> GetHitActorsInShape(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        List<ActorData> actorsInShape = AreaEffectUtils.GetActorsInShape(this.GetDestShape(), targets[0], base.IgnoreLos(), caster, TargeterUtils.GetRelevantTeams(caster, base.IncludeAllies(), base.IncludeEnemies()), nonActorTargetInfo);
        ServerAbilityUtils.RemoveEvadersFromHitTargets(ref actorsInShape);
        return actorsInShape;
    }

    public override List<ServerClientUtils.SequenceStartData> CreateSequenceStartData(List<AbilityTarget> targets, ActorData caster, ServerAbilityUtils.AbilityRunData additionalData, Sequence.IExtraSequenceParams[] extraSequenceParams = null)
    {
        List<ServerClientUtils.SequenceStartData> list = new List<ServerClientUtils.SequenceStartData>();
        if (this.m_bounceHitTargeting)
        {
            Vector3 loSCheckPos = caster.GetLoSCheckPos(caster.GetSquareAtPhaseStart());
            Vector3 item;
            List<ActorData> bounceHitActors = this.GetBounceHitActors(targets, loSCheckPos, caster, out item, null);
            List<Sequence.IExtraSequenceParams> list2 = new List<Sequence.IExtraSequenceParams>();
            if (extraSequenceParams != null)
            {
                list2.AddRange(extraSequenceParams);
            }
            BouncingShotSequence.ExtraParams extraParams = new BouncingShotSequence.ExtraParams();
            extraParams.laserTargets = new Dictionary<ActorData, AreaEffectUtils.BouncingLaserInfo>();
            foreach (ActorData key in bounceHitActors)
            {
                extraParams.laserTargets.Add(key, new AreaEffectUtils.BouncingLaserInfo(loSCheckPos, 0));
            }
            extraParams.segmentPts = new List<Vector3>
        {
            item
        };
            list2.Add(extraParams);
            ServerClientUtils.SequenceStartData item2 = new ServerClientUtils.SequenceStartData(this.m_chargeSequencePrefab, caster.GetCurrentBoardSquare(), new ActorData[0], caster, additionalData.m_sequenceSource, list2.ToArray());
            list.Add(item2);
            if (bounceHitActors.Count > 0)
            {
                List<ActorData> list3 = additionalData.m_abilityResults.HitActorList();
                foreach (ActorData item3 in bounceHitActors)
                {
                    list3.Remove(item3);
                }
                ServerClientUtils.SequenceStartData item4 = new ServerClientUtils.SequenceStartData(this.m_aoeSequencePrefab, caster.GetFreePos(), additionalData.m_abilityResults.HitActorsArray(), caster, additionalData.m_sequenceSource, null);
                list.Add(item4);
            }
        }
        else
        {
            BoardSquare square = Board.Get().GetSquare(targets[0].GridPos);
            ServerClientUtils.SequenceStartData item5 = new ServerClientUtils.SequenceStartData(this.m_castSequencePrefab, square, additionalData.m_abilityResults.HitActorsArray(), caster, additionalData.m_sequenceSource, null);
            list.Add(item5);
        }
        return list;
    }

    */
}
