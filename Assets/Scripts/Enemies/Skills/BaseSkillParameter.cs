using UnityEngine;
using System.Collections;

public class BaseSkillParameter : MonoBehaviour {

	public int id;

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

	public GhostLevelMaster.LevelPattern pattern = GhostLevelMaster.LevelPattern.normal;

	//TODO: in the future, it maybe developed ^^;
//	public bool isPercentage = false;

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

	public static void InstantilateStatusUpSkillEffects(GameObject character, int atkUp, int hpUp, float brightUp) {
		//atkup exists
		if (atkUp > 0) {
			GameObject atkUpEffectObj = (GameObject)Instantiate (Resources.Load("Prefabs/Particle/Skill/AtkUpParticle"));
			atkUpEffectObj.transform.parent = character.transform;
			atkUpEffectObj.transform.localPosition = new Vector3 (0.0f, 140.0f, 0.0f);
		}

		//hpup exists
		if (hpUp > 0) {
			GameObject hpUpEffectObj = (GameObject)Instantiate (Resources.Load("Prefabs/Particle/Skill/HpUpParticle"));
			hpUpEffectObj.transform.parent = character.transform;
			hpUpEffectObj.transform.localPosition = new Vector3 (0.0f, 140.0f, 0.0f);
		}

		//brightness exists
		if (brightUp > 0) {
			GameObject brightUpEffectObj = (GameObject)Instantiate (Resources.Load("Prefabs/Particle/Skill/BrightUpParticle"));
			brightUpEffectObj.transform.parent = character.transform;
			brightUpEffectObj.transform.localPosition = new Vector3 (0.0f, 140.0f, 0.0f);
		}
	}

	/**
	 * 
	 * Called when Unsummoning the NPC.
	 * 
	 */
	public static void RemoveAllSkillEffects(GameObject character) {
		if (character == null) {
			return;
		}

		ParticleSystem[] skillEffectArray = character.GetComponentsInChildren<ParticleSystem> ();
		foreach (ParticleSystem skillEffect in skillEffectArray) {
			Destroy (skillEffect.gameObject);
		}
	}
}
