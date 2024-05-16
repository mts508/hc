using AbilityContextNamespace;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelect_DualMeetingLasers : GenericAbility_TargetSelectBase
{
	public delegate int LaserCountDelegate(AbilityTarget currentTarget, ActorData caster);

	public delegate float ExtraAoeRadiusDelegate(AbilityTarget currentTarget, ActorData targetingActor, float baseRadius);

	[Separator("Targeting - Laser", true)]
	public float m_laserWidth = 0.5f;

	public float m_minMeetingDistFromCaster = 1f;

	public float m_maxMeetingDistFromCaster = 8f;

	public float m_laserStartForwardOffset;

	public float m_laserStartSideOffset = 0.5f;

	[Separator("Targeting - AoE", true)]
	public float m_aoeBaseRadius = 2.5f;

	public float m_aoeMinRadius;

	public float m_aoeMaxRadius = -1f;

	public float m_aoeRadiusChangePerUnitFromMin = 0.1f;

	[Header("-- Multiplier on radius if not all lasers meet")]
	public float m_radiusMultIfPartialBlock = 1f;

	[Space(10f)]
	public bool m_aoeIgnoreMinCoverDist = true;

	[Separator("Sequences", true)]
	public GameObject m_laserSequencePrefab;

	[Header("-- Use if laser doesn't have impact FX that spawns on end of laser, or for temp testing")]
	public GameObject m_aoeSequencePrefab;

	public LaserCountDelegate m_delegateLaserCount;

	public ExtraAoeRadiusDelegate m_delegateExtraAoeRadius;

	private TargetSelectMod_DualMeetingLasers m_targetSelMod;

	public override string GetUsageForEditor()
	{
		return GetContextUsageStr(ContextKeys.s_InAoe.GetName(), "on every hit actor, 1 if in AoE, 0 otherwise") + GetContextUsageStr(ContextKeys.s_DistFromMin.GetName(), "on every actor, distance of cursor pos from min distance, for interpolation");
	}

	public override void ListContextNamesForEditor(List<string> names)
	{
		names.Add(ContextKeys.s_InAoe.GetName());
		names.Add(ContextKeys.s_DistFromMin.GetName());
	}

	public override List<AbilityUtil_Targeter> CreateTargeters(Ability ability)
	{
		AbilityUtil_Targeter_ScampDualLasers abilityUtil_Targeter_ScampDualLasers = new AbilityUtil_Targeter_ScampDualLasers(ability, GetLaserWidth(), GetMinMeetingDistFromCaster(), GetMaxMeetingDistFromCaster(), GetLaserStartForwardOffset(), GetLaserStartSideOffset(), GetAoeBaseRadius(), GetAoeMinRadius(), GetAoeMaxRadius(), GetAoeRadiusChangePerUnitFromMin(), GetRadiusMultIfPartialBlock(), AoeIgnoreMinCoverDist());
		abilityUtil_Targeter_ScampDualLasers.SetAffectedGroups(IncludeEnemies(), IncludeAllies(), IncludeCaster());
		List<AbilityUtil_Targeter> list = new List<AbilityUtil_Targeter>();
		list.Add(abilityUtil_Targeter_ScampDualLasers);
		return list;
	}

	public override bool CanShowTargeterRangePreview(TargetData[] targetData)
	{
		return true;
	}

	public override float GetTargeterRangePreviewRadius(Ability ability, ActorData caster)
	{
		return GetMaxMeetingDistFromCaster() + GetAoeMinRadius();
	}

	protected override void OnTargetSelModApplied(TargetSelectModBase modBase)
	{
		m_targetSelMod = (modBase as TargetSelectMod_DualMeetingLasers);
	}

	protected override void OnTargetSelModRemoved()
	{
		m_targetSelMod = null;
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

	public float GetMinMeetingDistFromCaster()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_minMeetingDistFromCasterMod.GetModifiedValue(m_minMeetingDistFromCaster);
		}
		else
		{
			result = m_minMeetingDistFromCaster;
		}
		return result;
	}

	public float GetMaxMeetingDistFromCaster()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_maxMeetingDistFromCasterMod.GetModifiedValue(m_maxMeetingDistFromCaster);
		}
		else
		{
			result = m_maxMeetingDistFromCaster;
		}
		return result;
	}

	public float GetLaserStartForwardOffset()
	{
		return (m_targetSelMod == null) ? m_laserStartForwardOffset : m_targetSelMod.m_laserStartForwardOffsetMod.GetModifiedValue(m_laserStartForwardOffset);
	}

	public float GetLaserStartSideOffset()
	{
		return (m_targetSelMod == null) ? m_laserStartSideOffset : m_targetSelMod.m_laserStartSideOffsetMod.GetModifiedValue(m_laserStartSideOffset);
	}

	public float GetAoeBaseRadius()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_aoeBaseRadiusMod.GetModifiedValue(m_aoeBaseRadius);
		}
		else
		{
			result = m_aoeBaseRadius;
		}
		return result;
	}

	public float GetAoeMinRadius()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_aoeMinRadiusMod.GetModifiedValue(m_aoeMinRadius);
		}
		else
		{
			result = m_aoeMinRadius;
		}
		return result;
	}

	public float GetAoeMaxRadius()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_aoeMaxRadiusMod.GetModifiedValue(m_aoeMaxRadius);
		}
		else
		{
			result = m_aoeMaxRadius;
		}
		return result;
	}

	public float GetAoeRadiusChangePerUnitFromMin()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_aoeRadiusChangePerUnitFromMinMod.GetModifiedValue(m_aoeRadiusChangePerUnitFromMin);
		}
		else
		{
			result = m_aoeRadiusChangePerUnitFromMin;
		}
		return result;
	}

	public float GetRadiusMultIfPartialBlock()
	{
		return (m_targetSelMod == null) ? m_radiusMultIfPartialBlock : m_targetSelMod.m_radiusMultIfPartialBlockMod.GetModifiedValue(m_radiusMultIfPartialBlock);
	}

	public bool AoeIgnoreMinCoverDist()
	{
		return (m_targetSelMod == null) ? m_aoeIgnoreMinCoverDist : m_targetSelMod.m_aoeIgnoreMinCoverDistMod.GetModifiedValue(m_aoeIgnoreMinCoverDist);
	}

    public override void CalcHitTargets(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        this.ResetContextData();
        base.CalcHitTargets(targets, caster, nonActorTargetInfo);
        List<List<ActorData>> list;
        List<Vector3> list2;
        List<Vector3> list3;
        Vector3 aimAtPos;
        int num;
        float num2;
        List<ActorData> list4;
        this.GetHitActors(targets, caster, nonActorTargetInfo, out list, out list2, out list3, out aimAtPos, out num, out num2, out list4);
        float value = AbilityCommon_DualMeetingLasers.CalcMeetingPosDistFromMin(caster.GetLoSCheckPos(), aimAtPos, this.GetMinMeetingDistFromCaster());
        List<ActorData> list5 = new List<ActorData>();
        if (num >= 0)
        {
            foreach (ActorData actorData in list4)
            {
                this.AddHitActor(actorData, list3[num], this.AoeIgnoreMinCoverDist());
                base.SetActorContext(actorData, ContextKeys.s_InAoe.GetKey(), 1);
                base.SetActorContext(actorData, ContextKeys.s_DistFromMin.GetKey(), value);
                list5.Add(actorData);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            foreach (ActorData actorData2 in list[i])
            {
                if (!list5.Contains(actorData2))
                {
                    this.AddHitActor(actorData2, list2[i], false);
                    base.SetActorContext(actorData2, ContextKeys.s_InAoe.GetKey(), 0);
                    base.SetActorContext(actorData2, ContextKeys.s_DistFromMin.GetKey(), value);
                    list5.Add(actorData2);
                }
            }
        }
    }

    private void GetHitActors(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo, out List<List<ActorData>> laserHitActors, out List<Vector3> laserStartPosList, out List<Vector3> laserEndPosList, out Vector3 aimAtPos, out int aoeEndPosIndex, out float aoeRadius, out List<ActorData> aoeHitActors)
    {
        Vector3 loSCheckPos = caster.GetLoSCheckPos();
        AbilityTarget abilityTarget = targets[0];
        int num = 2;
        if (this.m_delegateLaserCount != null)
        {
            num = this.m_delegateLaserCount(targets[0], caster);
        }
        if (num > 1)
        {
            laserStartPosList = AbilityCommon_DualMeetingLasers.CalcStartingPositions(loSCheckPos, abilityTarget.FreePos, this.GetLaserStartForwardOffset(), this.GetLaserStartSideOffset());
        }
        else
        {
            laserStartPosList = new List<Vector3>
        {
            loSCheckPos
        };
        }
        aimAtPos = AbilityCommon_DualMeetingLasers.CalcClampedMeetingPos(loSCheckPos, abilityTarget.FreePos, this.GetMinMeetingDistFromCaster(), this.GetMaxMeetingDistFromCaster());
        float num2 = AbilityCommon_DualMeetingLasers.CalcAoeRadius(loSCheckPos, aimAtPos, this.GetAoeBaseRadius(), this.GetMinMeetingDistFromCaster(), this.GetAoeRadiusChangePerUnitFromMin(), this.GetAoeMinRadius(), this.GetAoeMaxRadius());
        if (this.m_delegateExtraAoeRadius != null)
        {
            num2 += this.m_delegateExtraAoeRadius(abilityTarget, caster, this.GetAoeBaseRadius());
        }
        aoeRadius = num2;
        AbilityCommon_DualMeetingLasers.CalcHitActors(aimAtPos, laserStartPosList, this.GetLaserWidth(), num2, this.GetRadiusMultIfPartialBlock(), caster, TargeterUtils.GetRelevantTeams(caster, base.IncludeAllies(), base.IncludeEnemies()), true, nonActorTargetInfo, out laserHitActors, out laserEndPosList, out aoeEndPosIndex, out aoeRadius, out aoeHitActors);
    }

    public override List<ServerClientUtils.SequenceStartData> CreateSequenceStartData(List<AbilityTarget> targets, ActorData caster, ServerAbilityUtils.AbilityRunData additionalData, Sequence.IExtraSequenceParams[] extraSequenceParams = null)
    {
        List<ServerClientUtils.SequenceStartData> list = new List<ServerClientUtils.SequenceStartData>();
        List<NonActorTargetInfo> nonActorTargetInfo = new List<NonActorTargetInfo>();
        List<List<ActorData>> list2;
        List<Vector3> list3;
        List<Vector3> list4;
        Vector3 vector;
        int num;
        float num2;
        List<ActorData> list5;
        this.GetHitActors(targets, caster, nonActorTargetInfo, out list2, out list3, out list4, out vector, out num, out num2, out list5);
        for (int i = 0; i < list3.Count; i++)
        {
            List<Sequence.IExtraSequenceParams> list6 = new List<Sequence.IExtraSequenceParams>();
            if (extraSequenceParams != null)
            {
                list6.AddRange(extraSequenceParams);
            }
            list6.Add(new SplineProjectileSequence.DelayedProjectileExtraParams
            {
                skipImpactFx = (i != num),
                useOverrideStartPos = true,
                overrideStartPos = list3[i]
            });
            if (i == 1)
            {
                list6.Add(new Sequence.GenericIntParam
                {
                    m_fieldIdentifier = Sequence.GenericIntParam.FieldIdentifier.Index,
                    m_value = 1
                });
            }
            List<ActorData> list7 = list2[i];
            if (i == num)
            {
                list6.Add(new Sequence.FxAttributeParam
                {
                    m_paramNameCode = Sequence.FxAttributeParam.ParamNameCode.ScaleControl,
                    m_paramTarget = Sequence.FxAttributeParam.ParamTarget.ImpactVfx,
                    m_paramValue = 2f * num2
                });
                if (this.m_aoeSequencePrefab == null)
                {
                    list7.AddRange(list5);
                }
            }
            Vector3 targetPos = list4[i];
            ServerClientUtils.SequenceStartData item = new ServerClientUtils.SequenceStartData(this.m_laserSequencePrefab, targetPos, list7.ToArray(), caster, additionalData.m_sequenceSource, list6.ToArray());
            list.Add(item);
        }
        if (num >= 0 && this.m_aoeSequencePrefab != null)
        {
            List<Sequence.IExtraSequenceParams> list8 = new List<Sequence.IExtraSequenceParams>();
            if (extraSequenceParams != null)
            {
                list8.AddRange(extraSequenceParams);
            }
            Sequence.IExtraSequenceParams[] adjustableRingSequenceParams = AbilityCommon_LayeredRings.GetAdjustableRingSequenceParams(num2);
            list8.AddRange(adjustableRingSequenceParams);
            ServerClientUtils.SequenceStartData item2 = new ServerClientUtils.SequenceStartData(this.m_aoeSequencePrefab, list4[num], list5.ToArray(), caster, additionalData.m_sequenceSource, list8.ToArray());
            list.Add(item2);
        }
        return list;
    }
}
