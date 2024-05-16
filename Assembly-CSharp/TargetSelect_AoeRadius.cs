using AbilityContextNamespace;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelect_AoeRadius : GenericAbility_TargetSelectBase
{
	[Separator("Targeting Properties", true)]
	public float m_radius = 1f;

	[Space(10f)]
	public bool m_useSquareCenterPos;

	[Separator("Sequences", true)]
	public GameObject m_castSequencePrefab;

	private TargetSelectMod_AoeRadius m_targetSelMod;

	public override string GetUsageForEditor()
	{
		return GetContextUsageStr(ContextKeys.s_DistFromStart.GetName(), "on every hit actor, distance from center of AoE, in squares");
	}

	public override void ListContextNamesForEditor(List<string> names)
	{
		names.Add(ContextKeys.s_DistFromStart.GetName());
	}

	public override void Initialize()
	{
		m_commonProperties.SetValue(ContextKeys.s_Radius.GetKey(), GetRadius());
	}

	public override List<AbilityUtil_Targeter> CreateTargeters(Ability ability)
	{
		AbilityUtil_Targeter_AoE_Smooth abilityUtil_Targeter_AoE_Smooth = new AbilityUtil_Targeter_AoE_Smooth(ability, GetRadius(), IgnoreLos());
		abilityUtil_Targeter_AoE_Smooth.SetAffectedGroups(IncludeEnemies(), IncludeAllies(), IncludeCaster());
		abilityUtil_Targeter_AoE_Smooth.m_customCenterPosDelegate = GetCenterPos;
		bool flag = ability.GetTargetData().Length == 0;
		abilityUtil_Targeter_AoE_Smooth.SetShowArcToShape(!flag);
		List<AbilityUtil_Targeter> list = new List<AbilityUtil_Targeter>();
		list.Add(abilityUtil_Targeter_AoE_Smooth);
		return list;
	}

	public Vector3 GetCenterPos(ActorData caster, AbilityTarget currentTarget)
	{
		if (UseSquareCenterPos())
		{
			BoardSquare boardSquareSafe = Board.Get().GetSquare(currentTarget.GridPos);
			if (boardSquareSafe != null)
			{
				while (true)
				{
					switch (4)
					{
					case 0:
						break;
					default:
						return boardSquareSafe.ToVector3();
					}
				}
			}
		}
		return currentTarget.FreePos;
	}

	public float GetRadius()
	{
		return (m_targetSelMod == null) ? m_radius : m_targetSelMod.m_radiusMod.GetModifiedValue(m_radius);
	}

	public bool UseSquareCenterPos()
	{
		bool result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_useSquareCenterPosMod.GetModifiedValue(m_useSquareCenterPos);
		}
		else
		{
			result = m_useSquareCenterPos;
		}
		return result;
	}

	public override bool CanShowTargeterRangePreview(TargetData[] targetData)
	{
		return true;
	}

	public override float GetTargeterRangePreviewRadius(Ability ability, ActorData caster)
	{
		return GetRadius();
	}

	protected override void OnTargetSelModApplied(TargetSelectModBase modBase)
	{
		m_targetSelMod = (modBase as TargetSelectMod_AoeRadius);
	}

	protected override void OnTargetSelModRemoved()
	{
		m_targetSelMod = null;
	}

    public override void CalcHitTargets(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        this.ResetContextData();
        base.CalcHitTargets(targets, caster, nonActorTargetInfo);
        this.m_contextCalcData.m_nonActorSpecificContext.SetValue(ContextKeys.s_Radius.GetKey(), this.GetRadius());
        Vector3 centerPos = this.GetCenterPos(caster, targets[0]);
        foreach (ActorData actorData in this.GetHitActors(targets, caster, nonActorTargetInfo))
        {
            this.AddHitActor(actorData, caster.GetLoSCheckPos(), false);
            float value = VectorUtils.HorizontalPlaneDistInSquares(centerPos, actorData.GetFreePos());
            base.SetActorContext(actorData, ContextKeys.s_DistFromStart.GetKey(), value);
        }
    }

    public List<ActorData> GetHitActors(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        List<ActorData> actorsInRadius = AreaEffectUtils.GetActorsInRadius(this.GetCenterPos(caster, targets[0]), this.GetRadius(), base.IgnoreLos(), caster, TargeterUtils.GetRelevantTeams(caster, base.IncludeAllies(), base.IncludeEnemies()), nonActorTargetInfo);
        if (base.IncludeCaster() && !actorsInRadius.Contains(caster))
        {
            actorsInRadius.Add(caster);
        }
        return actorsInRadius;
    }

    public override List<ServerClientUtils.SequenceStartData> CreateSequenceStartData(List<AbilityTarget> targets, ActorData caster, ServerAbilityUtils.AbilityRunData additionalData, Sequence.IExtraSequenceParams[] extraSequenceParams = null)
    {
        List<ServerClientUtils.SequenceStartData> list = new List<ServerClientUtils.SequenceStartData>();
        List<Sequence.IExtraSequenceParams> list2 = new List<Sequence.IExtraSequenceParams>();
        if (extraSequenceParams != null)
        {
            list2.AddRange(extraSequenceParams);
        }
        list2.AddRange(AbilityCommon_LayeredRings.GetAdjustableRingSequenceParams(this.GetRadius()));
        Vector3 centerPos = this.GetCenterPos(caster, targets[0]);
        ServerClientUtils.SequenceStartData item = new ServerClientUtils.SequenceStartData(this.m_castSequencePrefab, centerPos, additionalData.m_abilityResults.HitActorsArray(), caster, additionalData.m_sequenceSource, list2.ToArray());
        list.Add(item);
        return list;
    }
}
