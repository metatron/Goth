using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GhostLevelMaster {
	public const int BASE_COST = 30;

	public enum LevelPattern : int
	{
		normal,
		soujuku,
		bansei
	};

	public class RarityLevelMaster
	{
		public int rarity;
		public int maxLevel;
		public float compoCostRatio;
		public float compoExpRatio;

		public RarityLevelMaster(int rarity, int maxLevel, float compoCostRatio, float compoExpRatio) {
			this.rarity = rarity;
			this.maxLevel = maxLevel;
			this.compoCostRatio = compoCostRatio;
			this.compoExpRatio = compoExpRatio;
		}
	}

	//rarity, maxlevel, compoCostRatio, compoExpRatio
	public static Dictionary<int, RarityLevelMaster> RarityLevelMasterDic = new Dictionary<int, RarityLevelMaster>() {
		{1,  new RarityLevelMaster(1,  10,  0.5f,  1)},
		{2,  new RarityLevelMaster(2,  20,  0.5f,  5)},
		{3,  new RarityLevelMaster(3,  30,  0.75f, 10)},
		{4,  new RarityLevelMaster(4,  40,  1.0f,  15)},
		{5,  new RarityLevelMaster(5,  50,  1.5f,  20)},
		{6,  new RarityLevelMaster(6,  60,  4.0f,  30)},
		{7,  new RarityLevelMaster(7,  70,  7.0f,  40)},
		{8,  new RarityLevelMaster(8,  80,  8.0f,  50)},
		{9,  new RarityLevelMaster(9,  90,  9.0f,  60)},
		{10, new RarityLevelMaster(10, 100, 10.0f, 60)},
		{11, new RarityLevelMaster(11, 110, 11.0f, 60)}
	};



	/**
	 * 
	 * How to use:
	 * CalculateLevelParams(minAttack, maxAttack, crntLevel, maxLevel, pattern);
	 * CalculateLevelParams(minHealth, maxHealth, crntLevel, maxLevel, pattern);
	 * 
	 */
	public static int CalculateLevelParams(int min, int max, int level, int maxLevel, LevelPattern pattern) {
		float x = 0;
		switch (pattern) {
		case LevelPattern.soujuku:
			x = Mathf.Abs (Mathf.Log (level, maxLevel) - Mathf.Log (1, maxLevel));
			break;
		case LevelPattern.bansei:
			x = Mathf.Abs (Mathf.Log (maxLevel-level+1, maxLevel) - Mathf.Log (maxLevel, maxLevel));
			break;
		default:
			if (maxLevel != 1) {
				x = (float)(level - 1) / (float)(maxLevel - 1);
			}
			break;

		}

		return (int)(min+(max-min)*x);
	}

	public static float CalculateLevelParams(float min, float max, int level, int maxLevel, LevelPattern pattern) {
		float x = 0;
		switch (pattern) {
		case LevelPattern.soujuku:
			x = Mathf.Abs (Mathf.Log (level, maxLevel) - Mathf.Log (1, maxLevel));
			break;
		case LevelPattern.bansei:
			x = Mathf.Abs (Mathf.Log (maxLevel-level+1, maxLevel) - Mathf.Log (maxLevel, maxLevel));
			break;
		default:
			if (maxLevel != 1) {
				x = (float)(level - 1) / (float)(maxLevel - 1);
			}
			break;

		}

		return (min+(max-min)*x);
	}

	public static int CalculateCost(int cost, int ghostLevel, int rarity) {
		float rarityRatio = RarityLevelMasterDic [rarity].compoCostRatio;
		float compCost = (BASE_COST + cost) * (ghostLevel*2) * rarityRatio;
		return (int)compCost;
	}

	public static int GetMaxLevel(BaseParameterStatus status) {
		return RarityLevelMasterDic [status.rarity].maxLevel;
	}
}
