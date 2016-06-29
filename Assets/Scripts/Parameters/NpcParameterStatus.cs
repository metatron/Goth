using UnityEngine;
using System.Collections;

[System.Serializable]
public class NpcParameterStatus : BaseParameterStatus {

	[SerializeField]
	public int intimate;



	public static float MAX_SEARCHTIME_ONATTACK = 10.0f;
	public float crntSearchTime = 0.0f;

	//max distance bt. player.
	public static float MAX_SEARCHDISTANCE_ONATTACK = 3000.0f;

	//flag that currently searching target on attack
	//(if this flag is on, npc will not follow Player)
	public bool isSearchingOnAttack = false;




	/**
	 * 
	 * when enemy attacked Player, search for the enemy for certain amount of time.
	 * set this function on EnemyWeapon and Check4SlowMo.
	 * 
	 */
	public void BeginSearchEnemyOnAttack(EnemyWeapon enemyWeapon) {
		GameObject crntNpcObj = GameManager.Instance.crntNpcObj;
		if(crntNpcObj == null) {
			return ;
		}

		//set the target
		if (crntNpcObj.GetComponent<EnemyAI> ().target == null && !isSearchingOnAttack) {
			//init NPC param
			crntNpcObj.GetComponent<EnemyAI> ().target = enemyWeapon.owner.transform;
			crntNpcObj.GetComponent<EnemyAI> ().enemyMotion.targetPos = enemyWeapon.owner.transform;
			isSearchingOnAttack = true;
			crntSearchTime = 0.0f;
		}
		//if this value is on it wont attack
		crntNpcObj.GetComponent<EnemyAI> ().enemyMotion.isMotionStarted = false;

	}

	/**
	 * 
	 * update the time for searching.
	 * if time past or distanse over, reset the OnAttack Search.
	 * set this function on EnemyAi.
	 * 
	 */
	public void UpdateSearchEnemyOnAttack(float deltaTime) {
		GameObject crntNpcObj = GameManager.Instance.crntNpcObj;
		if(crntNpcObj == null || GameManager.Instance.player == null) {
			return ;
		}

		//if not searching, not necessary to do following
		if (!isSearchingOnAttack) {
			return ;
		}

		//if times up or dist bt. player is too far, go back to normal mode.
		float dist = Vector3.Distance (crntNpcObj.transform.position, GameManager.Instance.player.transform.position);
		if (crntSearchTime >= NpcParameterStatus.MAX_SEARCHTIME_ONATTACK || dist >= MAX_SEARCHDISTANCE_ONATTACK) {
			ResetSearchEnemyOnAttack();
		}

		crntSearchTime += deltaTime;
	}

	public void ResetSearchEnemyOnAttack() {
		isSearchingOnAttack = false;
		SelfObj.GetComponent<EnemyAI> ().target = null;
		SelfObj.GetComponent<EnemyAI> ().enemyMotion.targetPos = null;
	}

	/**
	 * 
	 * This is ONLY called from GhostNode.
	 * 
	 * 
	 */
	public void UpdateStatus(int level) {
		this.level = level;
		InitCharacterParameters (false); //do not instantilate effect

		//update close contact weapon atk. (if it is bullet type, no need)
		if (SelfObj != null) {
			GameObject closeContactWeapon = SelfObj.GetComponent<EnemyAI> ().enemyMotion.crntWeaponObj;
			if (closeContactWeapon != null) {
				closeContactWeapon.GetComponentInChildren<EnemyWeapon> ().attack = crntAtk;
			}
		}
	}

}
