using AbilityContextNamespace;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelect_ConeOrLaser : GenericAbility_TargetSelectBase
{
	[Separator("Targeting Properties", true)]
	public float m_coneDistThreshold = 4f;

	[Header("  Targeting: For Cone")]
	public ConeTargetingInfo m_coneInfo;

	[Header("  Targeting: For Laser")]
	public LaserTargetingInfo m_laserInfo;

	[Separator("Sequences", true)]
	public GameObject m_coneSequencePrefab;

	public GameObject m_laserSequencePrefab;

	public static ContextNameKeyPair s_cvarInCone = new ContextNameKeyPair("InCone");

	private TargetSelectMod_ConeOrLaser m_targetSelMod;

	private ConeTargetingInfo m_cachedConeInfo;

	private LaserTargetingInfo m_cachedLaserInfo;

	public override string GetUsageForEditor()
	{
		return GetContextUsageStr(ContextKeys.s_DistFromStart.GetName(), "distance from start of cone position, in squares") + GetContextUsageStr(s_cvarInCone.GetName(), "Whether the target hit is in cone") + GetContextUsageStr(ContextKeys.s_AngleFromCenter.GetName(), "angle from center of cone");
	}

	public override void ListContextNamesForEditor(List<string> keys)
	{
		keys.Add(ContextKeys.s_DistFromStart.GetName());
		keys.Add(s_cvarInCone.GetName());
		keys.Add(ContextKeys.s_AngleFromCenter.GetName());
	}

	public override void Initialize()
	{
		base.Initialize();
		SetCachedFields();
		ConeTargetingInfo coneInfo = GetConeInfo();
		coneInfo.m_affectsEnemies = IncludeEnemies();
		coneInfo.m_affectsAllies = IncludeAllies();
		coneInfo.m_affectsCaster = IncludeCaster();
		LaserTargetingInfo laserInfo = GetLaserInfo();
		laserInfo.affectsEnemies = IncludeEnemies();
		laserInfo.affectsAllies = IncludeAllies();
		laserInfo.affectsCaster = IncludeCaster();
	}

	public override List<AbilityUtil_Targeter> CreateTargeters(Ability ability)
	{
		AbilityUtil_Targeter_ConeOrLaser item = new AbilityUtil_Targeter_ConeOrLaser(ability, GetConeInfo(), GetLaserInfo(), GetConeDistThreshold());
		List<AbilityUtil_Targeter> list = new List<AbilityUtil_Targeter>();
		list.Add(item);
		return list;
	}

	public bool ShouldUseCone(Vector3 freePos, ActorData caster)
	{
		Vector3 vector = freePos - caster.GetFreePos();
		vector.y = 0f;
		float magnitude = vector.magnitude;
		return magnitude <= GetConeDistThreshold();
	}

	private void SetCachedFields()
	{
		ConeTargetingInfo cachedConeInfo;
		if (m_targetSelMod != null)
		{
			cachedConeInfo = m_targetSelMod.m_coneInfoMod.GetModifiedValue(m_coneInfo);
		}
		else
		{
			cachedConeInfo = m_coneInfo;
		}
		m_cachedConeInfo = cachedConeInfo;
		LaserTargetingInfo cachedLaserInfo;
		if (m_targetSelMod != null)
		{
			cachedLaserInfo = m_targetSelMod.m_laserInfoMod.GetModifiedValue(m_laserInfo);
		}
		else
		{
			cachedLaserInfo = m_laserInfo;
		}
		m_cachedLaserInfo = cachedLaserInfo;
	}

	public float GetConeDistThreshold()
	{
		return (m_targetSelMod == null) ? m_coneDistThreshold : m_targetSelMod.m_coneDistThresholdMod.GetModifiedValue(m_coneDistThreshold);
	}

	public ConeTargetingInfo GetConeInfo()
	{
		return (m_cachedConeInfo == null) ? m_coneInfo : m_cachedConeInfo;
	}

	public LaserTargetingInfo GetLaserInfo()
	{
		LaserTargetingInfo result;
		if (m_cachedLaserInfo != null)
		{
			result = m_cachedLaserInfo;
		}
		else
		{
			result = m_laserInfo;
		}
		return result;
	}

	protected override void OnTargetSelModApplied(TargetSelectModBase modBase)
	{
		m_targetSelMod = (modBase as TargetSelectMod_ConeOrLaser);
	}

	protected override void OnTargetSelModRemoved()
	{
		m_targetSelMod = null;
	}

	//rogues
    public override void CalcHitTargets(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        this.ResetContextData();
        base.CalcHitTargets(targets, caster, nonActorTargetInfo);
        bool useCone = this.ShouldUseCone(targets[0].FreePos, caster);
        Vector3 loSCheckPos = caster.GetLoSCheckPos();
        Vector3 vector;
        foreach (ActorData actor in this.GetHitActors(targets, caster, useCone, nonActorTargetInfo, out vector))
        {
            this.AddHitActor(actor, loSCheckPos, false);
            base.SetActorContext(actor, TargetSelect_ConeOrLaser.s_cvarInCone.GetKey(), useCone ? 1 : 0);
        }
    }

	//rogues
    private List<ActorData> GetHitActors(List<AbilityTarget> targets, ActorData caster, bool useCone, List<NonActorTargetInfo> nonActorTargetInfo, out Vector3 endPosIfLaser)
    {
        Vector3 loSCheckPos = caster.GetLoSCheckPos();
        float coneCenterAngleDegrees = VectorUtils.HorizontalAngle_Deg(targets[0].AimDirection);
        ConeTargetingInfo coneInfo = this.m_coneInfo;
        List<Team> affectedTeams = coneInfo.GetAffectedTeams(caster);
        List<ActorData> result;
        if (useCone)
        {
            result = AreaEffectUtils.GetActorsInCone(loSCheckPos, coneCenterAngleDegrees, coneInfo.m_widthAngleDeg, coneInfo.m_radiusInSquares, coneInfo.m_backwardsOffset, coneInfo.m_penetrateLos, caster, affectedTeams, nonActorTargetInfo);
            endPosIfLaser = targets[0].FreePos;
        }
        else
        {
            LaserTargetingInfo laserInfo = this.m_laserInfo;
            VectorUtils.LaserCoords laserCoords;
            laserCoords.start = loSCheckPos;
            result = AreaEffectUtils.GetActorsInLaser(laserCoords.start, targets[0].AimDirection, laserInfo.range, laserInfo.width, caster, affectedTeams, laserInfo.penetrateLos, laserInfo.maxTargets, false, true, out laserCoords.end, nonActorTargetInfo, null, false, true);
            endPosIfLaser = laserCoords.end;
        }
        return result;
    }

	//rogues
    public override List<ServerClientUtils.SequenceStartData> CreateSequenceStartData(List<AbilityTarget> targets, ActorData caster, ServerAbilityUtils.AbilityRunData additionalData, Sequence.IExtraSequenceParams[] extraSequenceParams = null)
    {
        List<ServerClientUtils.SequenceStartData> list = new List<ServerClientUtils.SequenceStartData>();
        bool flag = this.ShouldUseCone(targets[0].FreePos, caster);
        Vector3 targetPos;
        this.GetHitActors(targets, caster, flag, null, out targetPos);
        ConeTargetingInfo coneInfo = this.m_coneInfo;
        List<Sequence.IExtraSequenceParams> list2 = new List<Sequence.IExtraSequenceParams>();
        if (extraSequenceParams != null)
        {
            list2.AddRange(extraSequenceParams);
        }
        if (flag)
        {
            list2.Add(new BlasterStretchConeSequence.ExtraParams
            {
                forwardAngle = VectorUtils.HorizontalAngle_Deg(targets[0].AimDirection),
                angleInDegrees = coneInfo.m_widthAngleDeg,
                lengthInSquares = coneInfo.m_radiusInSquares
            });
        }
        ServerClientUtils.SequenceStartData item = new ServerClientUtils.SequenceStartData(flag ? this.m_coneSequencePrefab : this.m_laserSequencePrefab, targetPos, additionalData.m_abilityResults.HitActorsArray(), caster, additionalData.m_sequenceSource, list2.ToArray());
        list.Add(item);
        return list;
    }
}
