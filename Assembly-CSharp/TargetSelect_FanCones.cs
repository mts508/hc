using AbilityContextNamespace;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelect_FanCones : GenericAbility_TargetSelectBase
{
	[Separator("Targeting Properties", true)]
	public ConeTargetingInfo m_coneInfo;

	[Space(10f)]
	public int m_coneCount = 3;

	[Header("Starting offset, move towards forward/aim direction")]
	public float m_coneStartOffsetInAimDir;

	[Header("Starting offset, move towards left/right")]
	public float m_coneStartOffsetToSides;

	[Header("Starting offset, move towards each cone's direction")]
	public float m_coneStartOffsetInConeDir;

	[Header("-- If Fixed Angle")]
	public float m_angleInBetween = 10f;

	[Header("-- If Interpolating Angle")]
	public bool m_changeAngleByCursorDistance = true;

	public float m_targeterMinAngle;

	public float m_targeterMaxAngle = 180f;

	public float m_startAngleOffset;

	[Space(10f)]
	public float m_targeterMinInterpDistance = 0.5f;

	public float m_targeterMaxInterpDistance = 4f;

	[Separator("Sequences", true)]
	public GameObject m_castSequencePrefab;

	private TargetSelectMod_FanCones m_targetSelMod;

	private ConeTargetingInfo m_cachedConeInfo;

	public override string GetUsageForEditor()
	{
		return GetContextUsageStr(ContextKeys.s_HitCount.GetName(), "on every hit actor, number of cone hits on target");
	}

	public override void ListContextNamesForEditor(List<string> names)
	{
		names.Add(ContextKeys.s_HitCount.GetName());
	}

	public override void Initialize()
	{
		SetCachedFields();
		ConeTargetingInfo coneInfo = GetConeInfo();
		coneInfo.m_affectsAllies = IncludeAllies();
		coneInfo.m_affectsEnemies = IncludeEnemies();
		coneInfo.m_affectsCaster = IncludeCaster();
		coneInfo.m_penetrateLos = IgnoreLos();
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
	}

	public ConeTargetingInfo GetConeInfo()
	{
		return (m_cachedConeInfo == null) ? m_coneInfo : m_cachedConeInfo;
	}

	public int GetConeCount()
	{
		int result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_coneCountMod.GetModifiedValue(m_coneCount);
		}
		else
		{
			result = m_coneCount;
		}
		return result;
	}

	public float GetConeStartOffsetInAimDir()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_coneStartOffsetInAimDirMod.GetModifiedValue(m_coneStartOffsetInAimDir);
		}
		else
		{
			result = m_coneStartOffsetInAimDir;
		}
		return result;
	}

	public float GetConeStartOffsetToSides()
	{
		return (m_targetSelMod == null) ? m_coneStartOffsetToSides : m_targetSelMod.m_coneStartOffsetToSidesMod.GetModifiedValue(m_coneStartOffsetToSides);
	}

	public float GetConeStartOffsetInConeDir()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_coneStartOffsetInConeDirMod.GetModifiedValue(m_coneStartOffsetInConeDir);
		}
		else
		{
			result = m_coneStartOffsetInConeDir;
		}
		return result;
	}

	public float GetAngleInBetween()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_angleInBetweenMod.GetModifiedValue(m_angleInBetween);
		}
		else
		{
			result = m_angleInBetween;
		}
		return result;
	}

	public bool ChangeAngleByCursorDistance()
	{
		bool result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_changeAngleByCursorDistanceMod.GetModifiedValue(m_changeAngleByCursorDistance);
		}
		else
		{
			result = m_changeAngleByCursorDistance;
		}
		return result;
	}

	public float GetTargeterMinAngle()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_targeterMinAngleMod.GetModifiedValue(m_targeterMinAngle);
		}
		else
		{
			result = m_targeterMinAngle;
		}
		return result;
	}

	public float GetTargeterMaxAngle()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_targeterMaxAngleMod.GetModifiedValue(m_targeterMaxAngle);
		}
		else
		{
			result = m_targeterMaxAngle;
		}
		return result;
	}

	public float GetStartAngleOffset()
	{
		float result;
		if (m_targetSelMod != null)
		{
			result = m_targetSelMod.m_startAngleOffsetMod.GetModifiedValue(m_startAngleOffset);
		}
		else
		{
			result = m_startAngleOffset;
		}
		return result;
	}

	protected virtual bool UseCasterPosForLoS()
	{
		return false;
	}

	protected virtual bool CustomLoS(ActorData actor, ActorData caster)
	{
		return true;
	}

	public override List<AbilityUtil_Targeter> CreateTargeters(Ability ability)
	{
		AbilityUtil_Targeter_TricksterCones abilityUtil_Targeter_TricksterCones = new AbilityUtil_Targeter_TricksterCones(ability, GetConeInfo(), GetConeCount(), GetConeCount, GetConeOrigins, GetConeDirections, GetFreePosForAim, false, UseCasterPosForLoS());
		abilityUtil_Targeter_TricksterCones.m_customDamageOriginDelegate = GetDamageOriginForTargeter;
		List<AbilityUtil_Targeter> list = new List<AbilityUtil_Targeter>();
		list.Add(abilityUtil_Targeter_TricksterCones);
		return list;
	}

	private Vector3 GetDamageOriginForTargeter(AbilityTarget currentTarget, Vector3 defaultOrigin, ActorData actorToAdd, ActorData caster)
	{
		return caster.GetFreePos();
	}

	public Vector3 GetFreePosForAim(AbilityTarget currentTarget, ActorData caster)
	{
		return currentTarget.FreePos;
	}

	public virtual List<Vector3> GetConeOrigins(AbilityTarget currentTarget, Vector3 targeterFreePos, ActorData caster)
	{
		List<Vector3> list = new List<Vector3>();
		Vector3 travelBoardSquareWorldPositionForLos = caster.GetLoSCheckPos();
		Vector3 aimDirection = currentTarget.AimDirection;
		Vector3 normalized = Vector3.Cross(aimDirection, Vector3.up).normalized;
		int coneCount = GetConeCount();
		int num = coneCount / 2;
		bool flag = coneCount % 2 == 0;
		float num2 = GetConeStartOffsetInAimDir() * Board.SquareSizeStatic;
		float num3 = GetConeStartOffsetToSides() * Board.SquareSizeStatic;
		for (int i = 0; i < coneCount; i++)
		{
			Vector3 b = Vector3.zero;
			if (num2 != 0f)
			{
				b = num2 * aimDirection;
			}
			if (num3 > 0f)
			{
				if (flag)
				{
					if (i < num)
					{
						b -= (float)(num - i) * num3 * normalized;
					}
					else
					{
						b += (float)(i - num + 1) * num3 * normalized;
					}
				}
				else if (i < num)
				{
					b -= (float)(num - i) * num3 * normalized;
				}
				else if (i > num)
				{
					b += (float)(i - num) * num3 * normalized;
				}
			}
			list.Add(travelBoardSquareWorldPositionForLos + b);
		}
		if (GetConeStartOffsetInConeDir() > 0f)
		{
			List<Vector3> coneDirections = GetConeDirections(currentTarget, targeterFreePos, caster);
			float d = GetConeStartOffsetInConeDir() * Board.SquareSizeStatic;
			for (int j = 0; j < coneDirections.Count; j++)
			{
				list[j] += d * coneDirections[j];
			}
		}
		return list;
	}

	public virtual List<Vector3> GetConeDirections(AbilityTarget currentTarget, Vector3 targeterFreePos, ActorData caster)
	{
		List<Vector3> list = new List<Vector3>();
		float num = GetAngleInBetween();
		int coneCount = GetConeCount();
		if (ChangeAngleByCursorDistance())
		{
			float num2;
			if (coneCount > 1)
			{
				num2 = AbilityCommon_FanLaser.CalculateFanAngleDegrees(currentTarget, caster, GetTargeterMinAngle(), GetTargeterMaxAngle(), m_targeterMinInterpDistance, m_targeterMaxInterpDistance, 0f);
			}
			else
			{
				num2 = 0f;
			}
			float num3 = num2;
			float num4;
			if (coneCount > 1)
			{
				num4 = num3 / (float)(coneCount - 1);
			}
			else
			{
				num4 = 0f;
			}
			num = num4;
		}
		float num5 = VectorUtils.HorizontalAngle_Deg(currentTarget.AimDirection) + GetStartAngleOffset();
		float num6 = num5 - 0.5f * (float)(coneCount - 1) * num;
		for (int i = 0; i < coneCount; i++)
		{
			list.Add(VectorUtils.AngleDegreesToVector(num6 + (float)i * num));
		}
		return list;
	}

	protected override void OnTargetSelModApplied(TargetSelectModBase modBase)
	{
		m_targetSelMod = (modBase as TargetSelectMod_FanCones);
	}

	protected override void OnTargetSelModRemoved()
	{
		m_targetSelMod = null;
	}

    public override void CalcHitTargets(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        this.ResetContextData();
        base.CalcHitTargets(targets, caster, nonActorTargetInfo);
        List<List<ActorData>> list;
        List<Vector3> list2;
        List<Vector3> list3;
        int num;
        Dictionary<ActorData, int> hitActorsAndHitCount = this.GetHitActorsAndHitCount(targets, caster, out list, out list2, out list3, out num, nonActorTargetInfo);
        foreach (ActorData actorData in hitActorsAndHitCount.Keys)
        {
            this.AddHitActor(actorData, caster.GetLoSCheckPos(), false);
            base.SetActorContext(actorData, ContextKeys.s_HitCount.GetKey(), hitActorsAndHitCount[actorData]);
        }
    }

    protected Dictionary<ActorData, int> GetHitActorsAndHitCount(List<AbilityTarget> targets, ActorData caster, out List<List<ActorData>> actorsForSequence, out List<Vector3> coneEndPosList, out List<Vector3> coneStartPosList, out int numConesWithHits, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        actorsForSequence = new List<List<ActorData>>();
        coneEndPosList = new List<Vector3>();
        numConesWithHits = 0;
        Dictionary<ActorData, int> dictionary = new Dictionary<ActorData, int>();
        List<Vector3> coneDirections = this.GetConeDirections(targets[0], targets[0].FreePos, caster);
        List<Vector3> coneOrigins = this.GetConeOrigins(targets[0], targets[0].FreePos, caster);
        coneStartPosList = coneOrigins;
        ConeTargetingInfo coneInfo = this.m_coneInfo;
        for (int i = 0; i < coneDirections.Count; i++)
        {
            Vector3 vector = coneDirections[i];
            Vector3 vector2 = coneOrigins[i];
            List<ActorData> actorsInCone = AreaEffectUtils.GetActorsInCone(vector2, VectorUtils.HorizontalAngle_Deg(vector), coneInfo.m_widthAngleDeg, coneInfo.m_radiusInSquares, coneInfo.m_backwardsOffset, coneInfo.m_penetrateLos, caster, TargeterUtils.GetRelevantTeams(caster, coneInfo.m_affectsAllies, coneInfo.m_affectsEnemies), nonActorTargetInfo);
            if (coneInfo.m_affectsCaster && i == 0)
            {
                actorsInCone.Add(caster);
            }
            foreach (ActorData actorData in actorsInCone.ToArray())
            {
                if (!this.CustomLoS(actorData, caster))
                {
                    actorsInCone.Remove(actorData);
                }
            }
            actorsForSequence.Add(actorsInCone);
            coneEndPosList.Add(vector2 + coneInfo.m_radiusInSquares * Board.SquareSizeStatic * vector);
            if (actorsInCone.Count > 0)
            {
                numConesWithHits++;
            }
            foreach (ActorData actorData2 in actorsInCone)
            {
                if (dictionary.ContainsKey(actorData2))
                {
                    Dictionary<ActorData, int> dictionary2 = dictionary;
                    ActorData key = actorData2;
                    dictionary2[key]++;
                }
                else
                {
                    dictionary[actorData2] = 1;
                }
            }
        }
        return dictionary;
    }

    public override List<ServerClientUtils.SequenceStartData> CreateSequenceStartData(List<AbilityTarget> targets, ActorData caster, ServerAbilityUtils.AbilityRunData additionalData, Sequence.IExtraSequenceParams[] extraSequenceParams = null)
    {
        List<ServerClientUtils.SequenceStartData> list = new List<ServerClientUtils.SequenceStartData>();
        bool flag = false;
        if (additionalData.m_abilityResults.HitActorList().Contains(caster))
        {
            flag = true;
        }
        List<List<ActorData>> list2;
        List<Vector3> list3;
        List<Vector3> list4;
        int num;
        this.GetHitActorsAndHitCount(targets, caster, out list2, out list3, out list4, out num, null);
        for (int i = 0; i < list2.Count; i++)
        {
            List<Sequence.IExtraSequenceParams> list5 = new List<Sequence.IExtraSequenceParams>();
            if (extraSequenceParams != null)
            {
                list5.AddRange(extraSequenceParams);
            }
            Sequence.IExtraSequenceParams[] collection = this.CreateConeSequenceExtraParam(list4[i], list3[i]);
            list5.AddRange(collection);
            ServerClientUtils.SequenceStartData item = new ServerClientUtils.SequenceStartData(this.m_castSequencePrefab, list4[i], list2[i].ToArray(), caster, additionalData.m_sequenceSource, list5.ToArray());
            list.Add(item);
        }
        if (flag)
        {
            ServerClientUtils.SequenceStartData item2 = new ServerClientUtils.SequenceStartData(SequenceLookup.Get().GetSimpleHitSequencePrefab(), caster.GetFreePos(), caster.AsArray(), caster, additionalData.m_sequenceSource, null);
            list.Add(item2);
        }
        return list;
    }

    public virtual Sequence.IExtraSequenceParams[] CreateConeSequenceExtraParam(Vector3 coneStartPos, Vector3 coneEndPos)
    {
        BlasterStretchConeSequence.ExtraParams extraParams = new BlasterStretchConeSequence.ExtraParams();
        extraParams.lengthInSquares = this.m_coneInfo.m_radiusInSquares;
        extraParams.angleInDegrees = this.m_coneInfo.m_widthAngleDeg;
        float forwardAngle = VectorUtils.HorizontalAngle_Deg(coneEndPos - coneStartPos);
        extraParams.forwardAngle = forwardAngle;
        return extraParams.ToArray();
    }

	// TODO
	/*

    public override void CalcHitTargets(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        this.ResetContextData();
        base.CalcHitTargets(targets, caster, nonActorTargetInfo);
        Vector3 vector = caster.GetLoSCheckPos();
        Vector3 aimDirection = targets[0].AimDirection;
        if (!Mathf.Approximately(this.m_backwardsDistanceOffset, 0f))
        {
            vector = caster.GetLoSCheckPos() - aimDirection.normalized * this.m_backwardsDistanceOffset;
        }
        float coneCenterAngleDegrees = VectorUtils.HorizontalAngle_Deg(aimDirection);
        int numActiveLayers = this.GetNumActiveLayers();
        this.GetNonActorSpecificContext().SetValue(ContextKeys.s_LayersActive.GetKey(), numActiveLayers);
        foreach (ActorData actorData in this.GetConeHitActors(targets, caster, nonActorTargetInfo))
        {
            this.AddHitActor(actorData, vector, false);
            int num = 0;
            while (num < this.m_cachedRadiusList.Count && num < numActiveLayers)
            {
                if (AreaEffectUtils.IsSquareInConeByActorRadius(actorData.GetCurrentBoardSquare(), vector, coneCenterAngleDegrees, this.GetConeWidthAngle(), this.m_cachedRadiusList[num], 0f, base.IgnoreLos(), caster))
                {
                    base.SetActorContext(actorData, ContextKeys.s_Layer.GetKey(), num);
                    break;
                }
                num++;
            }
        }
    }

    private List<ActorData> GetConeHitActors(List<AbilityTarget> targets, ActorData caster, List<NonActorTargetInfo> nonActorTargetInfo)
    {
        Vector3 aimDirection = targets[0].AimDirection;
        float coneCenterAngleDegrees = VectorUtils.HorizontalAngle_Deg(aimDirection);
        Vector3 vector = caster.GetLoSCheckPos();
        if (!Mathf.Approximately(this.m_backwardsDistanceOffset, 0f))
        {
            vector = caster.GetLoSCheckPos() - aimDirection.normalized * this.m_backwardsDistanceOffset;
        }
        List<ActorData> actorsInCone = AreaEffectUtils.GetActorsInCone(vector, coneCenterAngleDegrees, this.GetConeWidthAngle(), this.GetMaxConeRadius(), 0f, base.IgnoreLos(), caster, TargeterUtils.GetRelevantTeams(caster, base.IncludeAllies(), base.IncludeEnemies()), nonActorTargetInfo);
        if (base.IncludeCaster() && !actorsInCone.Contains(caster))
        {
            actorsInCone.Add(caster);
        }
        TargeterUtils.SortActorsByDistanceToPos(ref actorsInCone, vector);
        return actorsInCone;
    }

    public override List<ServerClientUtils.SequenceStartData> CreateSequenceStartData(List<AbilityTarget> targets, ActorData caster, ServerAbilityUtils.AbilityRunData additionalData, Sequence.IExtraSequenceParams[] extraSequenceParams = null)
    {
        List<ServerClientUtils.SequenceStartData> list = new List<ServerClientUtils.SequenceStartData>();
        List<Sequence.IExtraSequenceParams> list2 = new List<Sequence.IExtraSequenceParams>();
        if (extraSequenceParams != null)
        {
            list2.AddRange(extraSequenceParams);
        }
        BlasterStretchConeSequence.ExtraParams extraParams = new BlasterStretchConeSequence.ExtraParams();
        extraParams.angleInDegrees = this.GetConeWidthAngle();
        extraParams.forwardAngle = VectorUtils.HorizontalAngle_Deg(targets[0].AimDirection);
        extraParams.lengthInSquares = this.GetMaxConeRadius();
        if (!Mathf.Approximately(this.m_backwardsDistanceOffset, 0f))
        {
            extraParams.useStartPosOverride = true;
            extraParams.startPosOverride = targets[0].FreePos - targets[0].AimDirection.normalized * this.m_backwardsDistanceOffset;
        }
        list2.Add(extraParams);
        ServerClientUtils.SequenceStartData item = new ServerClientUtils.SequenceStartData(this.m_coneSequencePrefab, caster.GetCurrentBoardSquare(), additionalData.m_abilityResults.HitActorsArray(), caster, additionalData.m_sequenceSource, list2.ToArray());
        list.Add(item);
        return list;
    }

    public float GetConeWidthAngle()
    {
        if (this.m_targetSelMod == null)
        {
            return this.m_coneWidthAngle;
        }
        return this.m_targetSelMod.m_coneWidthAngleMod.GetModifiedValue(this.m_coneWidthAngle);
    }

    public float GetMaxConeRadius()
    {
        float result = 0f;
        int numActiveLayers = this.GetNumActiveLayers();
        if (numActiveLayers > 0)
        {
            result = this.m_cachedRadiusList[numActiveLayers - 1];
        }
        return result;
    }

    public int GetNumActiveLayers()
    {
        if (this.m_delegateNumActiveLayers != null)
        {
            return this.m_delegateNumActiveLayers(this.m_cachedRadiusList.Count);
        }
        return this.m_cachedRadiusList.Count;
    }

	*/


}
