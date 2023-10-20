using AbilityContextNamespace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TargetSelect_Laser : GenericAbility_TargetSelectBase
{
	[Separator("Targeting Properties", true)]
	public float m_laserRange = 5f;

	public float m_laserWidth = 1f;

	public int m_maxTargets;

	[Separator("AoE around start", true)]
	public float m_aoeRadiusAroundStart;

	[Separator("Sequences", true)]
	public GameObject m_castSequencePrefab;

	public GameObject m_aoeAtStartSequencePrefab;

	private TargetSelectMod_Laser m_targetSelMod;


    public override string GetUsageForEditor()
	{
		return GetContextUsageStr(ContextKeys.s_HitOrder.GetName(), "on every non-caster hit actor, order in which they are hit in laser") + GetContextUsageStr(ContextKeys.s_DistFromStart.GetName(), "on every non-caster hit actor, distance from caster");
	}

	public override void ListContextNamesForEditor(List<string> names)
	{
		names.Add(ContextKeys.s_HitOrder.GetName());
		names.Add(ContextKeys.s_DistFromStart.GetName());
	}

	public override List<AbilityUtil_Targeter> CreateTargeters(Ability ability)
	{
		List<AbilityUtil_Targeter> list = new List<AbilityUtil_Targeter>();
		AbilityUtil_Targeter abilityUtil_Targeter;
		if (GetAoeRadiusAroundStart() <= 0f)
		{
			abilityUtil_Targeter = new AbilityUtil_Targeter_Laser(ability, GetLaserWidth(), GetLaserRange(), IgnoreLos(), GetMaxTargets(), IncludeAllies(), IncludeCaster());
		}
		else
		{
			abilityUtil_Targeter = new AbilityUtil_Targeter_ClaymoreSlam(ability, GetLaserRange(), GetLaserWidth(), GetMaxTargets(), 360f, GetAoeRadiusAroundStart(), 0f, IgnoreLos());
		}
		abilityUtil_Targeter.SetAffectedGroups(IncludeEnemies(), IncludeAllies(), IncludeCaster());
		list.Add(abilityUtil_Targeter);
		return list;
	}

	public float GetLaserRange()
	{
		return (m_targetSelMod == null) ? m_laserRange : m_targetSelMod.m_laserRangeMod.GetModifiedValue(m_laserRange);
	}

	public float GetLaserWidth()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_laserWidthMod.GetModifiedValue(m_laserWidth);
		}
		else
		{
			result = m_laserWidth;
		}
		return result;
	}

	public int GetMaxTargets()
	{
		int result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_maxTargetsMod.GetModifiedValue(m_maxTargets);
		}
		else
		{
			result = m_maxTargets;
		}
		return result;
	}

	public float GetAoeRadiusAroundStart()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_aoeRadiusAroundStartMod.GetModifiedValue(m_aoeRadiusAroundStart);
		}
		else
		{
			result = m_aoeRadiusAroundStart;
		}
		return result;
	}

	public override bool CanShowTargeterRangePreview(TargetData[] targetData)
	{
		return true;
	}

	public override float GetTargeterRangePreviewRadius(Ability ability, ActorData caster)
	{
		return GetLaserRange();
	}

	protected override void OnTargetSelModApplied(TargetSelectModBase modBase)
	{
		m_targetSelMod = (modBase as TargetSelectMod_Laser);
	}

	protected override void OnTargetSelModRemoved()
	{
		m_targetSelMod = null;
	}

	//rogues
    private void GetHitActors(List<AbilityTarget> targets, ActorData caster, out List<ActorData> actorsForSequence, out List<Vector3> targetPosForSequences, List<NonActorTargetInfo> nonActorTargetInfo, out Vector3 endPos)
    {
        actorsForSequence = new List<ActorData>();
        targetPosForSequences = new List<Vector3>();
        actorsForSequence = AreaEffectUtils.GetActorsInLaser(caster.GetLoSCheckPos(), targets[0].AimDirection, this.m_laserRange, this.m_laserWidth, caster, TargeterUtils.GetRelevantTeams(caster, this.m_includeAllies, this.m_includeEnemies), this.m_ignoreLos, this.m_maxTargets, false, true, out endPos, nonActorTargetInfo, null, false, true);
        targetPosForSequences.Add(endPos);
        if (actorsForSequence.Any<ActorData>())
        {
            Vector3 knockbackOriginFromLaser = AreaEffectUtils.GetKnockbackOriginFromLaser(actorsForSequence, caster, targets[0].AimDirection, endPos);
            this.GetNonActorSpecificContext().SetValue(ContextKeys.s_KnockbackOrigin.GetKey(), knockbackOriginFromLaser);
        }
        if (this.m_includeCaster && !actorsForSequence.Contains(caster))
        {
            actorsForSequence.Add(caster);
        }
    }

	//rogues
    public override void CalcHitTargets(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        this.ResetContextData();
        base.CalcHitTargets(targets, caster, nonActorTargetInfo);
        //if (this.m_maxPowerUpsGrabbedOnHit > 0)
        //{
        //    VectorUtils.LaserCoords laserCoords;
        //    List<PowerUp> list;
        //    List<ActorData> hitActorsInDirectionStatic = ThiefBasicAttack.GetHitActorsInDirectionStatic(caster.GetLoSCheckPos(), targets[0].AimDirection, caster, this.m_laserRange, this.m_laserWidth, this.m_ignoreLos, this.m_maxTargets, this.m_includeAllies, this.m_includeEnemies, true, this.m_maxPowerUpsGrabbedOnHit, true, this.m_stopOnPowerUp, this.m_includeSpoilsPowerUp, true, new HashSet<PowerUp>(), out laserCoords, out list, nonActorTargetInfo, false, true);
        //    if (list.Count > 0)
        //    {
        //        foreach (PowerUp powerUp in list)
        //        {
        //            this.m_powerUpsHit.Add(powerUp);
        //            powerUp.OnPickedUp(caster);
        //        }
        //        this.AddHitActor(caster, caster.GetFreePos(), false);
        //        base.SetActorContext(caster, TargetSelect_Laser.s_PowerUpsHit.GetKey(), list.Count);
        //    }
        //    for (int i = 0; i < hitActorsInDirectionStatic.Count; i++)
        //    {
        //        ActorData actor = hitActorsInDirectionStatic[i];
        //        this.AddHitActor(actor, caster.GetLoSCheckPos(), false);
        //        base.SetActorContext(actor, ContextKeys.s_HitOrder.GetKey(), i);
        //    }
        //    return;
        //}
        List<ActorData> actorsForSequence;
        List<Vector3> targetPosForSequence;
        Vector3 endPos;
        this.GetHitActors(targets, caster, out actorsForSequence, out targetPosForSequence, nonActorTargetInfo, out endPos);
        for (int actorIndex = 0; actorIndex < actorsForSequence.Count; actorIndex++)
        {
            ActorData actor = actorsForSequence[actorIndex];
            this.AddHitActor(actor, caster.GetLoSCheckPos(), false);
            base.SetActorContext(actor, ContextKeys.s_HitOrder.GetKey(), actorIndex);
        }
    }

    public override List<ServerClientUtils.SequenceStartData> CreateSequenceStartData(List<AbilityTarget> targets, ActorData caster, ServerAbilityUtils.AbilityRunData additionalData, Sequence.IExtraSequenceParams[] extraSequenceParams = null)
    {
        List<ServerClientUtils.SequenceStartData> list = new List<ServerClientUtils.SequenceStartData>();
        //if (this.m_maxPowerUpsGrabbedOnHit > 0)
        //{
        //    List<Vector3> list2;
        //    List<List<ActorData>> list3;
        //    List<List<PowerUp>> list4;
        //    this.GetSequencePositionAndTargetsWithPowerups(targets, caster, out list2, out list3, out list4);
        //    for (int i = 0; i < list2.Count; i++)
        //    {
        //        if (list4[i].Count > 0 && this.m_powerupReturnPrefab != null)
        //        {
        //            list3[i].Remove(caster);
        //        }
        //        ServerClientUtils.SequenceStartData item = new ServerClientUtils.SequenceStartData(this.m_castSequencePrefab, list2[i], list3[i].ToArray(), caster, additionalData.m_sequenceSource, extraSequenceParams);
        //        list.Add(item);
        //        if (list4[i].Count > 0 && this.m_powerupReturnPrefab != null)
        //        {
        //            List<PowerUp> list5 = list4[i];
        //            for (int j = 0; j < list5.Count; j++)
        //            {
        //                PowerUp powerUp = list5[j];
        //                List<Sequence.IExtraSequenceParams> list6 = new List<Sequence.IExtraSequenceParams>();
        //                list6.Add(new SplineProjectileSequence.DelayedProjectileExtraParams
        //                {
        //                    useOverrideStartPos = true,
        //                    overrideStartPos = powerUp.gameObject.transform.position
        //                });
        //                list6.Add(new ThiefPowerupReturnProjectileSequence.PowerupTypeExtraParams
        //                {
        //                    powerupCategory = (int)powerUp.m_chatterCategory
        //                });
        //                ServerClientUtils.SequenceStartData item2 = new ServerClientUtils.SequenceStartData(this.m_powerupReturnPrefab, caster.GetFreePos(), caster.AsArray(), caster, additionalData.m_sequenceSource, list6.ToArray());
        //                list.Add(item2);
        //            }
        //        }
        //    }
        //}
        //else
        //{
        List<ActorData> actorHit;
        List<Vector3> targetPosForSequences;
        Vector3 endPos;

        this.GetHitActors(targets, caster, out actorHit, out targetPosForSequences, null, out endPos);
        list.Add(new ServerClientUtils.SequenceStartData(this.m_castSequencePrefab, Board.Get().GetSquareFromVec3(endPos), actorHit.ToArray(), caster, additionalData.m_sequenceSource, extraSequenceParams));
        //}
        return list;
    }
}
