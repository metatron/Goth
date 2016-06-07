using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseParameterStatus {
	private static List<string> GhostPrefabPathList = new List<string>(){
		"Prefabs/Enemies/TiedGhost",
		"Prefabs/Enemies/NurseGhost",
		"Prefabs/Enemies/WillOWisp",
		"Prefabs/Enemies/Queen"
	};

	public enum GhostType : int
	{
		TiedGhost,
		Nurse,
		WillOWisp,
		Queen,
		Player = 99
	};

	public enum CharacterDirection : int {
		LEFT,
		RIGHT, 
		None = 99
	};


	public GameObject SelfObj { get; set; }

	[SerializeField]
	public int minHp = 10;
	[SerializeField]
	public int maxHp = 100;

	public int crntHp = 10;


	[SerializeField]
	public int minAtk = 10;
	[SerializeField]
	public int maxAtk = 100;

	public int crntAtk = 10;


	[SerializeField]
	public int minMoveSpeed = 100;
	[SerializeField]
	public int maxMoveSpeed = 200;

	public int crntMoveSpeed = 10;

	[SerializeField]
	public float minAtkSpeed = 0.0f;
	[SerializeField]
	public float maxAtkSpeed = 1.0f;

	public float crntAtkSpeed = 0.0f;

	[SerializeField]
	public float minVisibleInc = 0.0f; //distance added by the level up
	[SerializeField]
	public float maxVisibleInc = 100.0f; //distance added by the level up
	public float crntVisibleInc = 0.0f;

	[SerializeField]
	public int level = 1;

	[SerializeField]
	public GhostType type = GhostType.TiedGhost;

	[SerializeField]
	public GhostLevelMaster.LevelPattern pattern = GhostLevelMaster.LevelPattern.normal;

	[SerializeField]
	public int rarity = 1;

	[SerializeField]
	public int cost = 100;

	[SerializeField]
	public List<BaseSkillParameter.SkillType> skillList = new List<BaseSkillParameter.SkillType>();


	public void InitCharacterParameterNums() {
		crntHp = GetBaseHp ();
		crntAtk = GetBaseAtk ();
		crntMoveSpeed = GetBaseMoveSpeed ();
		crntAtkSpeed = GetBaseAtkSpeed ();
	}

	/**
	 * 
	 * loading prefabs of the ghost type.
	 * 
	 */
	public GameObject GetPrefab() {
		return (GameObject)Resources.Load(BaseParameterStatus.GhostPrefabPathList[(int)type]);
	}

	public CharacterDirection GetCharacterDirection() {
		if (SelfObj.transform.localScale.x < 0) {
			return CharacterDirection.RIGHT;
		}
		return CharacterDirection.LEFT;
	}


	virtual public void PrintParam() {
		Debug.LogError ("*********" + 
			"maxHp: " + maxHp + ", crntHp: " + GetBaseHp() + 
			", maxAtk: " + maxAtk + ", crntAtk: " + GetBaseAtk() + 
			", maxMoveSpeed: " + maxMoveSpeed + ", crntMoveSpeed: " + GetBaseMoveSpeed() + 
			", maxAtkSpeed: " + maxAtkSpeed + ", crntAtkSpeed: " + GetBaseAtkSpeed() + 
			", level: " + level + 
			", type: " + type
		);
	}

	public int GetBaseAtk() {
		return GhostLevelMaster.CalculateLevelParams (minAtk, maxAtk, level, GhostLevelMaster.GetMaxLevel(this), pattern);
	}
	public int GetBaseHp() {
		return GhostLevelMaster.CalculateLevelParams (minHp, maxHp, level, GhostLevelMaster.GetMaxLevel(this), pattern);
	}
	public int GetBaseMoveSpeed() {
		return GhostLevelMaster.CalculateLevelParams (minMoveSpeed, maxMoveSpeed, level, GhostLevelMaster.GetMaxLevel(this), pattern);
	}
	public float GetBaseAtkSpeed() {
		return GhostLevelMaster.CalculateLevelParams (minAtkSpeed, maxAtkSpeed, level, GhostLevelMaster.GetMaxLevel(this), pattern);
	}
	public int GetBaseVisibleDistance() {
		return GhostLevelMaster.CalculateLevelParams ((int)minVisibleDistance, (int)maxVisibleDistance, level, GhostLevelMaster.GetMaxLevel (this), pattern);
	}
}
