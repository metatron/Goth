using UnityEngine;
using System.Collections;

public class BaseSkillParameter : MonoBehaviour {

	public enum SkillType : int
	{
		NONE,

		ATKUP,
		HPUP,
		BRIGHTNESS,
		SHIELD,
		ATTACKALL
	};


	//ATK increase
	public int minPlayerAtkUp;
	public int maxPlayerAtkUp;

	//HP increase
	public int minPlayerHpUp;
	public int maxPlayerHpUp;

	//Brightness increase (0.0f - 1.0f)
	public float minBrightnessInc;
	public float maxBrightnessInc;

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

	public int GetBaseSkillPlayerAtkUp(BaseParameterStatus status) {
		return GhostLevelMaster.CalculateLevelParams (minPlayerAtkUp, maxPlayerAtkUp, status.level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public int GetBaseSkillPlayerHpUp(BaseParameterStatus status) {
		return GhostLevelMaster.CalculateLevelParams (minPlayerHpUp, maxPlayerHpUp, status.level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public float GetBasedSkillPlayerBrightUp(BaseParameterStatus status) {
		return GhostLevelMaster.CalculateLevelParams (minBrightnessInc, maxBrightnessInc, status.level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public int GetBaseSkillShieldNum(BaseParameterStatus status) {
		return GhostLevelMaster.CalculateLevelParams (minNumOfShield, maxNumOfShield, status.level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
	public int GetBaseSkillBombNum(BaseParameterStatus status) {
		return GhostLevelMaster.CalculateLevelParams (minNumOfBomb, maxNumOfBomb, status.level, GhostLevelMaster.GetMaxLevel(status), pattern);
	}
}
