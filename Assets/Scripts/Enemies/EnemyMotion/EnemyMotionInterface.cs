using UnityEngine;
using System.Collections;
using SmoothMoves;

public class EnemyMotionInterface : MonoBehaviour {
	public bool isMotionStarted = false; //during the attack, true.

	public Transform targetPos;

	public GameObject closeWeaponPrefab; //set only for close contact attackers
	public GameObject crntWeaponObj;

	virtual public void InitCloseContactWeapon(Vector3 initPos) {
		if (closeWeaponPrefab != null) {
			//if initializing npc, it is done after initializing enemy. crntWeaponObj is not null
			//1. init on CloseContactAttacker
			//2. npc init on GameManager.
			if(crntWeaponObj == null) {
				crntWeaponObj = (GameObject)Instantiate(closeWeaponPrefab);
				crntWeaponObj.transform.parent = GetComponent<BoneAnimation>().GetSpriteTransform("weapon");
				crntWeaponObj.GetComponentInChildren<EnemyWeapon>().owner = GetComponent<EnemyAI>().gameObject;
			}
			crntWeaponObj.transform.localPosition = initPos;
			crntWeaponObj.GetComponentInChildren<EnemyWeapon>().attack = GetComponent<EnemyAI>().status.crntAtk;

			//add to the enemy weapon list
			GameManager.Instance.enemyWeaponList.Add(crntWeaponObj.GetComponentInChildren<EnemyWeapon>());
		}
	}

	virtual public void InitEnemyMotion() {
		//disabling Weapon collider
		if (GetCollider () != null) {
			GetCollider ().enabled = false;
		}
	}

	/**
	 * 
	 * this is called from EnemyAI.Attack
	 * 
	 * 
	 */
	virtual public IEnumerator DoAttack() {
		return null;
	}

	/**
	 * 
	 * the place of this is called varies on each casses.
	 * 
	 * 
	 */
	virtual public void AttackFinished() {
	}

	/**
	 * 
	 * Getting Weapon Collider.
	 * 
	 */
	virtual public Collider GetCollider() {
		return null;
	}
}
