using UnityEngine;
using System.Collections;

public class BaseSkillParameter : MonoBehaviour {

	public enum SkillType : int
	{
		NONE,

		ATKUP,
		HPUP,
		VISIBILITYUP,
		SHIELD,
		ATTACKALL
	};


	public int level;

	//ATK increase
	public int minPlayerAtkUp;
	public int maxPlayerAtkUp;

	//HP increase
	public int minPlayerHpUp;
	public int maxPlayerHpUp;

	//Visibility increase
	public float minVisibility;
	public float maxVisibility;

	//Shield effect
	public int minNumOfShield;
	public int maxNumOfShield;
	public int numShieldUsed; //increment if the user has been damaged. cannot use if numUsed == crntNumObShield

	//ATTACK ALL effect
	public int minNumOfBomb;
	public int maxNumOfBomb;
	public int numBombUsed; //increment if the user has been damaged. cannot use if numUsed == crntNumObShield

	public SkillType skillType = SkillType.NONE;

	public GhostLevelMaster.LevelPattern pattern = GhostLevelMaster.LevelPattern.normal;

	public GameObject effect;

	//EnemyAI's GameObject.
	private GameObject selfObj;
	private BaseParameterStatus status;

	public void InitSkill(GameObject selfObj) {
		this.selfObj = selfObj;
		status = selfObj.GetComponent<EnemyAI> ().GetStatus ();
	}

	public int GetBaseAtkUp() {
		return GhostLevelMaster.CalculateLevelParams (minPlayerAtkUp, maxPlayerAtkUp, level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public int GetBaseHpUp() {
		return GhostLevelMaster.CalculateLevelParams (minPlayerHpUp, maxPlayerHpUp, level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public int GetBasedVisiUp() {
		return GhostLevelMaster.CalculateLevelParams (minVisibility, maxVisibility, level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public float GetBaseVisibleDistance() {
		return GhostLevelMaster.CalculateLevelParams (minVisibility, maxVisibility, level, GhostLevelMaster.GetMaxLevel (status), pattern);
	}
	public int GetBaseShieldNum() {
		return GhostLevelMaster.CalculateLevelParams (minNumOfShield, maxNumOfShield, level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public int GetBaseBombNum() {
		return GhostLevelMaster.CalculateLevelParams (minNumOfBomb, maxNumOfBomb, level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
}
