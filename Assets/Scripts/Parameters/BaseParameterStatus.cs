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
	public List<string> skillPathList = new List<string>();
	//
	private static Dictionary<string, BaseSkillParameter> skillPrefabDict = new Dictionary<string, BaseSkillParameter>();


	/**
	 * 
	 * Called ONLY for NPC related function.
	 * 
	 * from:
	 *  GameManager.SummonNpc
	 *  GhostNode.LevelUpCharacter -> npcStatus.UpdateStatus
	 * 
	 * The enemy status will be set from either from Spawner or the EventData.
	 * This function will not be used.
	 * 
	 * 
	 */
	public void InitCharacterParameters() {
		crntHp = GetBaseHp ();
		crntAtk = GetBaseAtk ();
		crntMoveSpeed = GetBaseMoveSpeed ();
		crntAtkSpeed = GetBaseAtkSpeed ();
		crntVisibleInc = GetBaseVisibleInc ();

		//Update with skills
		UpdatePlayerStatusOnSkill();
	}


	/**
	 * 
	 * the skill is affected on NPC Summoning, Unsummoning, and Stage Init.
	 * Called on:
	 * InitCharacterParameters
	 * InstantiateStage
	 * 
	 * 
	 * 
	 */
	public void UpdatePlayerStatusOnSkill() {
		if (GameManager.Instance.player == null) {
			return;
		}

		//update the player status
		int atkUp = 0;
		int hpUp = 0;
		float brightnessUp = 0;
		foreach (string skillpath in skillPathList) {
			//load resources and push it into Hashtable
			BaseSkillParameter skillPref = null;
			if (skillPrefabDict.ContainsKey (skillpath)) {
				skillPref = skillPrefabDict [skillpath];
			}
			//load from resources and put it into buffer
			else {
				GameObject skilltmp = (GameObject)Resources.Load (skillpath) as GameObject;
				skillPref = skilltmp.GetComponent<BaseSkillParameter> ();
				skillPrefabDict.Add(skillpath, skillPref);
			}

			atkUp			+= skillPref.GetBaseSkillPlayerAtkUp(this);
			hpUp		 	+= skillPref.GetBaseSkillPlayerHpUp(this);
			brightnessUp	+= skillPref.GetBasedSkillPlayerBrightUp(this);
		}

		//update player status
		GameManager.Instance.playerParam.crntAtk = GameManager.Instance.playerParam.GetBaseAtk () + atkUp;
		GameManager.Instance.playerParam.crntHp = GameManager.Instance.playerParam.GetBaseHp () + hpUp;

		//update brightness of the stage.
		StageManager.Instance.UpdateStageBrightnessBySkill (brightnessUp);
	}

	/**
	 * 
	 * loading prefabs of the ghost type.
	 * 
	 */
	public GameObject GetPrefab() {
		return (GameObject)Resources.Load(BaseParameterStatus.GhostPrefabPathList[(int)type]);
	}

	/**
	 * 
	 * used in Editor
	 */
	public static GameObject GetPrefabByType(GhostType type) {
		return (GameObject)Resources.Load(BaseParameterStatus.GhostPrefabPathList[(int)type]);
	}


	public CharacterDirection GetCharacterDirection() {
		if (SelfObj.transform.localScale.x < 0) {
			return CharacterDirection.RIGHT;
		}
		return CharacterDirection.LEFT;
	}


	virtual public void PrintParam() {
		Debug.LogError (SelfObj.GetComponent<EnemyAI>() + " *********" + 
			"crntHp: " + crntHp + 
			", crntAtk: " + crntAtk + 
			", crntMoveSpeed: " + crntMoveSpeed + 
			", crntAtkSpeed: " + crntAtkSpeed + 
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
	public int GetBaseVisibleInc() {
		return GhostLevelMaster.CalculateLevelParams ((int)minVisibleInc, (int)maxVisibleInc, level, GhostLevelMaster.GetMaxLevel (this), pattern);
	}
}
