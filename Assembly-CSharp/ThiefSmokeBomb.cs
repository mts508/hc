using System.Collections.Generic;
using UnityEngine;

public class ThiefSmokeBomb : Ability
{
	[Header("-- Bomb Damage")]
	public int m_extraDamageOnCast;

	[Header("-- Bomb Targeting (shape is in Smoke Field Info)")]
	public bool m_penetrateLos;

	public int m_maxAngleWithFirstSegment;

	public float m_maxDistanceWithFirst;

	public float m_minDistanceBetweenBombs = 1f;

	[Header("-- On Cast Hit Effect")]
	public StandardEffectInfo m_bombHitEffectInfo;

	[Header("-- Smoke Field")]
	public GroundEffectField m_smokeFieldInfo;

	[Header("-- Barrier (will make square out of 4 barriers around ground field)")]
	public bool m_addBarriers = true;

	public float m_barrierSquareWidth = 3f;

	public StandardBarrierData m_barrierData;

	[Header("-- Sequences")]
	public GameObject m_castSequencePrefab;

	private AbilityMod_ThiefSmokeBomb m_abilityMod;

	private StandardEffectInfo m_cachedBombHitEffectInfo;

	private GroundEffectField m_cachedSmokeFieldInfo;

	private StandardBarrierData m_cachedBarrierData;

	private void Start()
	{
		if (m_abilityName == "Base Ability")
		{
			m_abilityName = "Smoke Bomb";
		}
		if (m_barrierSquareWidth <= 0f)
		{
			Debug.LogWarning("Thief Smoke Bomb, Barrier Data has 0 width, setting to 3");
			m_barrierSquareWidth = 3f;
		}
		Setup();
	}

	private void Setup()
	{
		SetCachedFields();
		AbilityAreaShape shape = GetSmokeFieldInfo().shape;
		GroundEffectField fieldData = GetSmokeFieldInfo();
		if (GetExpectedNumberOfTargeters() > 1)
		{
			while (true)
			{
				switch (7)
				{
				case 0:
					break;
				default:
				{
					ClearTargeters();
					for (int i = 0; i < GetExpectedNumberOfTargeters(); i++)
					{
						bool penetrateLoS = PenetrateLos();
						int affectsAllies;
						if (fieldData.healAmount > 0)
						{
							affectsAllies = ((!fieldData.ignoreNonCasterAllies) ? 1 : 0);
						}
						else
						{
							affectsAllies = 0;
						}
						AbilityUtil_Targeter_Shape abilityUtil_Targeter_Shape = new AbilityUtil_Targeter_Shape(this, shape, penetrateLoS, AbilityUtil_Targeter_Shape.DamageOriginType.CenterOfShape, true, (byte)affectsAllies != 0);
						abilityUtil_Targeter_Shape.m_affectCasterDelegate = delegate(ActorData caster, List<ActorData> actorsSoFar, bool casterInShape)
						{
							int result2;
							if (fieldData.healAmount > 0)
							{
								result2 = (casterInShape ? 1 : 0);
							}
							else
							{
								result2 = 0;
							}
							return (byte)result2 != 0;
						};
						abilityUtil_Targeter_Shape.SetTooltipSubjectTypes();
						base.Targeters.Add(abilityUtil_Targeter_Shape);
					}
					while (true)
					{
						switch (4)
						{
						default:
							return;
						case 0:
							break;
						}
					}
				}
				}
			}
		}
		bool penetrateLoS2 = PenetrateLos();
		int affectsAllies2;
		if (fieldData.healAmount > 0)
		{
			affectsAllies2 = ((!fieldData.ignoreNonCasterAllies) ? 1 : 0);
		}
		else
		{
			affectsAllies2 = 0;
		}
		AbilityUtil_Targeter_Shape abilityUtil_Targeter_Shape2 = new AbilityUtil_Targeter_Shape(this, shape, penetrateLoS2, AbilityUtil_Targeter_Shape.DamageOriginType.CenterOfShape, true, (byte)affectsAllies2 != 0);
		abilityUtil_Targeter_Shape2.m_affectCasterDelegate = delegate(ActorData caster, List<ActorData> actorsSoFar, bool casterInShape)
		{
			int result;
			if (fieldData.healAmount > 0)
			{
				result = (casterInShape ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		};
		base.Targeter = abilityUtil_Targeter_Shape2;
	}

	public override int GetExpectedNumberOfTargeters()
	{
		return Mathf.Max(1, GetNumTargets());
	}

	private void SetCachedFields()
	{
		StandardEffectInfo cachedBombHitEffectInfo;
		if ((bool)m_abilityMod)
		{
			cachedBombHitEffectInfo = m_abilityMod.m_bombHitEffectInfoMod.GetModifiedValue(m_bombHitEffectInfo);
		}
		else
		{
			cachedBombHitEffectInfo = m_bombHitEffectInfo;
		}
		m_cachedBombHitEffectInfo = cachedBombHitEffectInfo;
		GroundEffectField cachedSmokeFieldInfo;
		if ((bool)m_abilityMod)
		{
			cachedSmokeFieldInfo = m_abilityMod.m_smokeFieldInfoMod.GetModifiedValue(m_smokeFieldInfo);
		}
		else
		{
			cachedSmokeFieldInfo = m_smokeFieldInfo;
		}
		m_cachedSmokeFieldInfo = cachedSmokeFieldInfo;
		StandardBarrierData cachedBarrierData;
		if ((bool)m_abilityMod)
		{
			cachedBarrierData = m_abilityMod.m_barrierDataMod.GetModifiedValue(m_barrierData);
		}
		else
		{
			cachedBarrierData = m_barrierData;
		}
		m_cachedBarrierData = cachedBarrierData;
	}

	public int GetExtraDamageOnCast()
	{
		int result;
		if ((bool)m_abilityMod)
		{
			result = m_abilityMod.m_extraDamageOnCastMod.GetModifiedValue(m_extraDamageOnCast);
		}
		else
		{
			result = m_extraDamageOnCast;
		}
		return result;
	}

	public bool PenetrateLos()
	{
		bool result;
		if ((bool)m_abilityMod)
		{
			result = m_abilityMod.m_penetrateLosMod.GetModifiedValue(m_penetrateLos);
		}
		else
		{
			result = m_penetrateLos;
		}
		return result;
	}

	public int GetMaxAngleWithFirstSegment()
	{
		return (!m_abilityMod) ? m_maxAngleWithFirstSegment : m_abilityMod.m_maxAngleWithFirstSegmentMod.GetModifiedValue(m_maxAngleWithFirstSegment);
	}

	public float GetMaxDistanceWithFirst()
	{
		float result;
		if ((bool)m_abilityMod)
		{
			result = m_abilityMod.m_maxDistanceWithFirstMod.GetModifiedValue(m_maxDistanceWithFirst);
		}
		else
		{
			result = m_maxDistanceWithFirst;
		}
		return result;
	}

	public float GetMinDistanceBetweenBombs()
	{
		float result;
		if ((bool)m_abilityMod)
		{
			result = m_abilityMod.m_minDistanceBetweenBombsMod.GetModifiedValue(m_minDistanceBetweenBombs);
		}
		else
		{
			result = m_minDistanceBetweenBombs;
		}
		return result;
	}

	public StandardEffectInfo GetBombHitEffectInfo()
	{
		StandardEffectInfo result;
		if (m_cachedBombHitEffectInfo != null)
		{
			result = m_cachedBombHitEffectInfo;
		}
		else
		{
			result = m_bombHitEffectInfo;
		}
		return result;
	}

	public GroundEffectField GetSmokeFieldInfo()
	{
		GroundEffectField result;
		if (m_cachedSmokeFieldInfo != null)
		{
			result = m_cachedSmokeFieldInfo;
		}
		else
		{
			result = m_smokeFieldInfo;
		}
		return result;
	}

	public bool AddBarriers()
	{
		return (!m_abilityMod) ? m_addBarriers : m_abilityMod.m_addBarriersMod.GetModifiedValue(m_addBarriers);
	}

	public float GetBarrierSquareWidth()
	{
		float result;
		if ((bool)m_abilityMod)
		{
			result = m_abilityMod.m_barrierSquareWidthMod.GetModifiedValue(m_barrierSquareWidth);
		}
		else
		{
			result = m_barrierSquareWidth;
		}
		return result;
	}

	public StandardBarrierData GetBarrierData()
	{
		StandardBarrierData result;
		if (m_cachedBarrierData != null)
		{
			result = m_cachedBarrierData;
		}
		else
		{
			result = m_barrierData;
		}
		return result;
	}

	public override bool CustomTargetValidation(ActorData caster, AbilityTarget target, int targetIndex, List<AbilityTarget> currentTargets)
	{
		Board board = Board.Get();
		BoardSquare boardSquareSafe = board.GetSquare(target.GridPos);
		Vector3 firstSegEndPos;
		AbilityAreaShape shape;
		bool flag;
		if (!(boardSquareSafe == null))
		{
			if (boardSquareSafe.IsValidForGameplay())
			{
				if (targetIndex == 0)
				{
					if (boardSquareSafe == caster.GetCurrentBoardSquare())
					{
						goto IL_0066;
					}
				}
				Vector3 vector = boardSquareSafe.ToVector3();
				firstSegEndPos = ((targetIndex <= 0) ? vector : board.GetSquare(currentTargets[0].GridPos).ToVector3());
				shape = GetSmokeFieldInfo().shape;
				flag = true;
				if (targetIndex > 0)
				{
					Vector3 to = vector - caster.GetFreePos();
					to.y = 0f;
					bool flag2 = true;
					if (GetMaxAngleWithFirstSegment() > 0)
					{
						BoardSquare boardSquareSafe2 = Board.Get().GetSquare(currentTargets[0].GridPos);
						Vector3 centerOfShape = AreaEffectUtils.GetCenterOfShape(shape, currentTargets[0].FreePos, boardSquareSafe2);
						Vector3 from = centerOfShape - caster.GetFreePos();
						from.y = 0f;
						int num = Mathf.RoundToInt(Vector3.Angle(from, to));
						flag2 = (num <= GetMaxAngleWithFirstSegment());
					}
					Vector3 centerOfShape2 = AreaEffectUtils.GetCenterOfShape(shape, vector, boardSquareSafe);
					Vector3 vector2 = centerOfShape2 - caster.GetFreePos();
					vector2.y = 0f;
					float magnitude = vector2.magnitude;
					int num2;
					if (!(GetMaxDistanceWithFirst() <= 0f))
					{
						num2 = ((magnitude <= GetMaxDistanceWithFirst() * board.squareSize) ? 1 : 0);
					}
					else
					{
						num2 = 1;
					}
					bool flag3 = (byte)num2 != 0;
					if (flag2)
					{
						if (flag3)
						{
							goto IL_01f2;
						}
					}
					flag = false;
				}
				goto IL_01f2;
			}
		}
		goto IL_0066;
		IL_01f2:
		if (flag)
		{
			float shapeCenterMinDistInWorld = 0.71f * board.squareSize;
			float minDistInWorld = GetMinDistanceBetweenBombs() * board.squareSize;
			for (int i = 0; i < targetIndex; i++)
			{
				if (flag)
				{
					BoardSquare boardSquareSafe3 = board.GetSquare(currentTargets[i].GridPos);
					Vector3 centerOfShape3 = AreaEffectUtils.GetCenterOfShape(shape, currentTargets[i].FreePos, boardSquareSafe3);
					flag = CheckMinDistConstraint(centerOfShape3, boardSquareSafe, shape, shapeCenterMinDistInWorld, minDistInWorld);
					continue;
				}
				break;
			}
			int expectedNumberOfTargeters = GetExpectedNumberOfTargeters();
			if (flag && targetIndex < expectedNumberOfTargeters - 1)
			{
				List<AbilityTarget> list = new List<AbilityTarget>();
				for (int j = 0; j < expectedNumberOfTargeters; j++)
				{
					list.Add(target.GetCopy());
				}
				for (int k = 0; k < targetIndex; k++)
				{
					list[k].SetPosAndDir(currentTargets[k].GridPos, currentTargets[k].FreePos, Vector3.forward);
				}
				list[targetIndex].SetPosAndDir(boardSquareSafe.GetGridPos(), target.FreePos, Vector3.forward);
				float currentRangeInSquares = AbilityUtils.GetCurrentRangeInSquares(this, caster, 0);
				flag = CanTargetFutureClicks(caster, firstSegEndPos, targetIndex, list, targetIndex, expectedNumberOfTargeters, currentRangeInSquares);
			}
		}
		return flag;
		IL_0066:
		return false;
	}

	public bool CanTargetFutureClicks(ActorData caster, Vector3 firstSegEndPos, int lastSelectedTargetIndex, List<AbilityTarget> targetEntries, int numTargetsFromPlayerInput, int numClicks, float abilityMaxRange)
	{
		if (lastSelectedTargetIndex >= numClicks - 1)
		{
			while (true)
			{
				switch (3)
				{
				case 0:
					break;
				default:
					return true;
				}
			}
		}
		Vector3 vec = firstSegEndPos - caster.GetFreePos();
		float coneWidthDegrees = Mathf.Min(360f, 2f * (float)GetMaxAngleWithFirstSegment() + 25f);
		AreaEffectUtils.GetMaxConeBounds(caster.GetFreePos(), VectorUtils.HorizontalAngle_Deg(vec), coneWidthDegrees, abilityMaxRange, 0f, out int minX, out int maxX, out int minY, out int maxY);
		Board board = Board.Get();
		AbilityAreaShape shape = GetSmokeFieldInfo().shape;
		AbilityData abilityData = caster.GetAbilityData();
		BoardSquare currentBoardSquare = caster.GetCurrentBoardSquare();
		float shapeCenterMinDistInWorld = 0.71f * board.squareSize;
		float minDistInWorld = GetMinDistanceBetweenBombs() * board.squareSize;
		bool flag = false;
		for (int i = minX; i < maxX; i++)
		{
			if (!flag)
			{
				for (int j = minY; j < maxY; j++)
				{
					if (!flag)
					{
						BoardSquare boardSquare = board.GetSquareFromIndex(i, j);
						if (!(boardSquare != null))
						{
							continue;
						}
						if (!boardSquare.IsValidForGameplay())
						{
							continue;
						}
						if (!currentBoardSquare.GetLOS(boardSquare.x, boardSquare.y))
						{
							continue;
						}
						if (!abilityData.IsTargetSquareInRangeOfAbilityFromSquare(boardSquare, currentBoardSquare, abilityMaxRange, 0f))
						{
							continue;
						}
						Vector3 vector = boardSquare.ToVector3();
						bool flag2 = true;
						bool flag3 = true;
						int maxAngleWithFirstSegment = GetMaxAngleWithFirstSegment();
						if (maxAngleWithFirstSegment > 0)
						{
							BoardSquare boardSquareSafe = Board.Get().GetSquare(targetEntries[0].GridPos);
							if (numTargetsFromPlayerInput > 0)
							{
								Vector3 centerOfShape = AreaEffectUtils.GetCenterOfShape(shape, targetEntries[0].FreePos, boardSquareSafe);
								Vector3 from = centerOfShape - caster.GetFreePos();
								Vector3 to = vector - caster.GetFreePos();
								int num = Mathf.RoundToInt(Vector3.Angle(from, to));
								flag3 = (num <= maxAngleWithFirstSegment);
							}
							else
							{
								for (int k = 0; k < 4; k++)
								{
									if (!flag2)
									{
										break;
									}
									Vector3 freePos = boardSquareSafe.ToVector3();
									freePos += 0.1f * VectorUtils.AngleDegreesToVector(45f + (float)k * 90f);
									Vector3 centerOfShape2 = AreaEffectUtils.GetCenterOfShape(shape, freePos, boardSquareSafe);
									Vector3 from2 = centerOfShape2 - caster.GetFreePos();
									Vector3 to2 = vector - caster.GetFreePos();
									int num2 = Mathf.RoundToInt(Vector3.Angle(from2, to2));
									int num3;
									if (flag3)
									{
										num3 = ((num2 <= maxAngleWithFirstSegment) ? 1 : 0);
									}
									else
									{
										num3 = 0;
									}
									flag3 = ((byte)num3 != 0);
								}
							}
						}
						Vector3 centerOfShape3 = AreaEffectUtils.GetCenterOfShape(shape, vector, boardSquare);
						Vector3 vector2 = centerOfShape3 - caster.GetFreePos();
						vector2.y = 0f;
						float magnitude = vector2.magnitude;
						int num4;
						if (!(GetMaxDistanceWithFirst() <= 0f))
						{
							num4 = ((magnitude <= GetMaxDistanceWithFirst() * board.squareSize) ? 1 : 0);
						}
						else
						{
							num4 = 1;
						}
						bool flag4 = (byte)num4 != 0;
						if (!flag3 || !flag4)
						{
							flag2 = false;
						}
						if (flag2)
						{
							for (int l = 0; l <= lastSelectedTargetIndex; l++)
							{
								if (flag2)
								{
									BoardSquare boardSquareSafe2 = board.GetSquare(targetEntries[l].GridPos);
									if (boardSquareSafe2 == boardSquare)
									{
										flag2 = false;
										continue;
									}
									if (l < numTargetsFromPlayerInput)
									{
										Vector3 freePos2 = targetEntries[l].FreePos;
										Vector3 centerOfShape4 = AreaEffectUtils.GetCenterOfShape(shape, freePos2, boardSquareSafe2);
										flag2 = CheckMinDistConstraint(centerOfShape4, boardSquare, shape, shapeCenterMinDistInWorld, minDistInWorld);
										continue;
									}
									for (int m = 0; m < 4; m++)
									{
										if (flag2)
										{
											Vector3 freePos3 = boardSquareSafe2.ToVector3();
											freePos3 += 0.1f * VectorUtils.AngleDegreesToVector(45f + (float)m * 90f);
											Vector3 centerOfShape5 = AreaEffectUtils.GetCenterOfShape(shape, freePos3, boardSquareSafe2);
											flag2 = CheckMinDistConstraint(centerOfShape5, boardSquare, shape, shapeCenterMinDistInWorld, minDistInWorld);
											continue;
										}
										break;
									}
									continue;
								}
								break;
							}
						}
						if (flag2)
						{
							if (lastSelectedTargetIndex < numClicks - 1)
							{
								targetEntries[lastSelectedTargetIndex + 1].SetPosAndDir(boardSquare.GetGridPos(), vector, Vector3.forward);
								flag2 = CanTargetFutureClicks(caster, firstSegEndPos, lastSelectedTargetIndex + 1, targetEntries, numTargetsFromPlayerInput, numClicks, abilityMaxRange);
							}
						}
						flag = flag2;
						continue;
					}
					break;
				}
				continue;
			}
			break;
		}
		return flag;
	}

	private bool CheckMinDistConstraint(Vector3 centerOfShapePrev, BoardSquare candidateSquare, AbilityAreaShape fieldShape, float shapeCenterMinDistInWorld, float minDistInWorld)
	{
		bool result = true;
		Vector3 vector = candidateSquare.ToVector3();
		int num = 0;
		while (true)
		{
			if (num < 4)
			{
				Vector3 freePos = vector;
				freePos += 0.1f * VectorUtils.AngleDegreesToVector(45f + (float)num * 90f);
				Vector3 centerOfShape = AreaEffectUtils.GetCenterOfShape(fieldShape, freePos, candidateSquare);
				Vector3 vector2 = centerOfShape - centerOfShapePrev;
				vector2.y = 0f;
				float magnitude = vector2.magnitude;
				if (!(magnitude < shapeCenterMinDistInWorld))
				{
					if (!(minDistInWorld > 0f))
					{
						goto IL_00a7;
					}
					if (!(magnitude < minDistInWorld))
					{
						goto IL_00a7;
					}
				}
				result = false;
			}
			else
			{
			}
			break;
			IL_00a7:
			num++;
		}
		return result;
	}

	protected override List<AbilityTooltipNumber> CalculateAbilityTooltipNumbers()
	{
		List<AbilityTooltipNumber> numbers = new List<AbilityTooltipNumber>();
		AbilityTooltipHelper.ReportDamage(ref numbers, AbilityTooltipSubject.Enemy, 1);
		AbilityTooltipHelper.ReportHealing(ref numbers, AbilityTooltipSubject.Ally, GetSmokeFieldInfo().healAmount);
		AbilityTooltipHelper.ReportHealing(ref numbers, AbilityTooltipSubject.Self, GetSmokeFieldInfo().healAmount);
		return numbers;
	}

	public override Dictionary<AbilityTooltipSymbol, int> GetCustomNameplateItemTooltipValues(ActorData targetActor, int currentTargeterIndex)
	{
		Dictionary<AbilityTooltipSymbol, int> symbolToValue = new Dictionary<AbilityTooltipSymbol, int>();
		BoardSquare boardSquareSafe = Board.Get().GetSquare(base.Targeters[0].LastUpdatingGridPos);
		int damageAmount = GetSmokeFieldInfo().damageAmount;
		int subsequentDamageAmount = GetSmokeFieldInfo().subsequentDamageAmount;
		for (int i = 0; i <= currentTargeterIndex; i++)
		{
			if (i > 0)
			{
				BoardSquare boardSquareSafe2 = Board.Get().GetSquare(base.Targeters[i].LastUpdatingGridPos);
				if (boardSquareSafe2 == null)
				{
					continue;
				}
				if (boardSquareSafe2 == boardSquareSafe)
				{
					continue;
				}
			}
			Ability.AddNameplateValueForOverlap(ref symbolToValue, base.Targeters[i], targetActor, currentTargeterIndex, damageAmount + GetExtraDamageOnCast(), subsequentDamageAmount);
		}
		while (true)
		{
			return symbolToValue;
		}
	}

	protected override void AddSpecificTooltipTokens(List<TooltipTokenEntry> tokens, AbilityMod modAsBase)
	{
		AbilityMod_ThiefSmokeBomb abilityMod_ThiefSmokeBomb = modAsBase as AbilityMod_ThiefSmokeBomb;
		string empty = string.Empty;
		int val;
		if ((bool)abilityMod_ThiefSmokeBomb)
		{
			val = abilityMod_ThiefSmokeBomb.m_extraDamageOnCastMod.GetModifiedValue(m_extraDamageOnCast);
		}
		else
		{
			val = m_extraDamageOnCast;
		}
		AddTokenInt(tokens, "ExtraDamageOnCast", empty, val);
		string empty2 = string.Empty;
		int val2;
		if ((bool)abilityMod_ThiefSmokeBomb)
		{
			val2 = abilityMod_ThiefSmokeBomb.m_maxAngleWithFirstSegmentMod.GetModifiedValue(m_maxAngleWithFirstSegment);
		}
		else
		{
			val2 = m_maxAngleWithFirstSegment;
		}
		AddTokenInt(tokens, "MaxAngleWithFirstSegment", empty2, val2);
		StandardEffectInfo effectInfo;
		if ((bool)abilityMod_ThiefSmokeBomb)
		{
			effectInfo = abilityMod_ThiefSmokeBomb.m_bombHitEffectInfoMod.GetModifiedValue(m_bombHitEffectInfo);
		}
		else
		{
			effectInfo = m_bombHitEffectInfo;
		}
		AbilityMod.AddToken_EffectInfo(tokens, effectInfo, "BombHitEffectInfo", m_bombHitEffectInfo);
		StandardBarrierData standardBarrierData;
		if ((bool)abilityMod_ThiefSmokeBomb)
		{
			standardBarrierData = abilityMod_ThiefSmokeBomb.m_barrierDataMod.GetModifiedValue(m_barrierData);
		}
		else
		{
			standardBarrierData = m_barrierData;
		}
		StandardBarrierData standardBarrierData2 = standardBarrierData;
		standardBarrierData2.AddTooltipTokens(tokens, "BarrierData", abilityMod_ThiefSmokeBomb != null, m_barrierData);
	}

	protected override void OnApplyAbilityMod(AbilityMod abilityMod)
	{
		if (abilityMod.GetType() != typeof(AbilityMod_ThiefSmokeBomb))
		{
			return;
		}
		while (true)
		{
			m_abilityMod = (abilityMod as AbilityMod_ThiefSmokeBomb);
			Setup();
			return;
		}
	}

	protected override void OnRemoveAbilityMod()
	{
		m_abilityMod = null;
		Setup();
	}
}
