using UnityEngine;
using System.Collections;
using SmoothMoves;

[RequireComponent (typeof (Rigidbody))]
public class EnemyWeapon : MonoBehaviour {
	public GameObject owner; //EnemyAI
	public int attack;
//	public EnemyWeaponType crntEnemyWeaponType = EnemyWeaponType.ENEMY;
	public Xft.XWeaponTrail trailObject;

	public float time2Destroy = 0f;
//	public bool destroyOnHit = false; //if it hits target destroy collider and rigitbody it instantly (use it on bullet)

	private GameObject hitParticlePrefab;

	public enum EnemyWeaponType: int {
		ENEMY,
		NPC
	}


	public void SetTime2Destroy(float time) {
		StartCoroutine (DestroyWeapon(time));
	}

	private IEnumerator DestroyWeapon(float time) {
		yield return new WaitForSeconds(time);
		if (trailObject != null) {
			trailObject.DestroyMeshObj ();
		}

		//remove from GameManager
		for(int i=0; i<GameManager.Instance.enemyWeaponList.Count; i++) {
			EnemyWeapon removeObj = GameManager.Instance.enemyWeaponList [i];
			//compare w/ reference. if true, remove index.
			if (removeObj.Equals (this)) {
				GameManager.Instance.enemyWeaponList.RemoveAt(i);
				break;
			}
		}

		Destroy (gameObject);
	}

	public EnemyWeaponType GetEnemyWeaponType() {
		if (owner == null) {
			return EnemyWeaponType.ENEMY;
		}

		//if owner isn't set, return enemy
		 EnemyAI enemyAi = owner.GetComponent<EnemyAI> ();
		if (enemyAi == null) {
			return EnemyWeaponType.ENEMY;
		}

		if(enemyAi.charType == EnemyAI.CharacterType.NPC) {
			return EnemyWeaponType.NPC;
		}
		return EnemyWeaponType.ENEMY;
	}


	void OnTriggerEnter(Collider other) {
		PlayerCharacter player = other.GetComponent<PlayerCharacter> ();

		//if NPC weapon hit w/ player, ignore all the below.
		EnemyWeaponType crntEnemyWeaponType = GetEnemyWeaponType ();
		if (crntEnemyWeaponType == EnemyWeaponType.NPC && player != null) {
			return;
		}

		//right before dmg check if the owner still exists
		if (owner == null) {
			//if owner is dead befer animating, isHit will not be able to return to false.
			PlayerParameterStatus.isHit = false;
			return;
		}


		BaseParameterStatus status = null;
		//1. hitting player
		if (player != null && player.status != null) {
			PlayerParameterStatus.isHit = true;
			status = player.status;
		}

		//2. hitting enemy (enemy or npc)
		EnemyAI target = other.gameObject.GetComponent<EnemyAI> ();
		if (target != null) {
			status = target.status;

			//if npc weapon hitting npc, do not dmg
			if (target.charType == EnemyAI.CharacterType.NPC && crntEnemyWeaponType == EnemyWeaponType.NPC) {
				return;
			}

			//if enemy weapon hitting npc, do nothing
			if (target.charType == EnemyAI.CharacterType.NPC && crntEnemyWeaponType == EnemyWeaponType.ENEMY) {
				return;
			}
			//if enemy weapon hitting enemy, do nothing
			if (target.charType == EnemyAI.CharacterType.ENEMY && crntEnemyWeaponType == EnemyWeaponType.ENEMY) {
				return;
			}
		}

		//. hitting other stuff (player weapon, etc)
		if (status == null) {
			return;
		}


		//generate particle
		if (hitParticlePrefab == null) {
			hitParticlePrefab = Resources.Load ("Prefabs/Battle/Effect/BloodEmitter") as GameObject;
		}
		GameObject hitParticleObj = (GameObject) Instantiate (hitParticlePrefab);
		Vector3 hitPosition = other.ClosestPointOnBounds(hitParticleObj.transform.position);
		//up a little bit
		hitParticleObj.transform.position = new Vector3 (hitPosition.x, hitPosition.y + 100.0f, hitPosition.z);

		//test output
		if (owner != null && (owner.GetComponent<EnemyAI> ().status.GetType () == typeof(NpcParameterStatus))) {
			Debug.LogError (owner.name + " attack: " + attack);
		}

		status.crntHp -= attack;

		//start animate damage
		if (player != null) {
			player.GetComponent<PlayerCharacter> ().playerAnimation.Play ("damage");
			player.GetComponent<PlayerCharacter> ().playerAnimation.PlayQueued ("stand");
		}


		//disable collider (and rigidbody)
//		if (destroyOnHit) {
			GetComponent<Collider> ().enabled = false;
//		}

		//TODO create damage animation for Charlotte
//		BoneAnimation enemyAnim = other.gameObject.GetComponent<BoneAnimation> ();
//		//Play damage animation
//		if (enemyAnim != null && !enemyAnim.IsPlaying ("damage")) {
//			enemyAnim.Play ("damage");
//			enemyAnim.PlayQueued("normal");
//		}




		//if player is dmgs, shake camera
		//(avoid double shake)
		if (Camera.main.GetComponent<iTween> () == null) {
			iTween.ShakePosition (Camera.main.gameObject,
				iTween.Hash (
					"amount", new Vector3(20f, 20f, 0f),
					"islocal", false,
					"time", 0.2f
				));
		}

		//back off target by dmg
		Hashtable paramTable = new Hashtable ();
		paramTable.Add ("status", status);
		GameManager.PushedByDamage (owner.transform, status.SelfObj.transform, gameObject, "AfterDamageProcess", paramTable);
	}

	private void AfterDamageProcess(object paramObject) {
		//if the enemy attack mostion is started but got damage, cancel the attack.
		if (owner.GetComponent<EnemyAI> ().enemyMotion.isMotionStarted) {
			owner.GetComponent<EnemyAI> ().enemyMotion.AttackFinished();
		}



		//player can input again
		PlayerParameterStatus.isHit = false;

		if (paramObject == null)
			return;

		//getting target status param from iTween hash.
		Hashtable paramTable = (Hashtable)paramObject;
		BaseParameterStatus status = null;
		if (paramTable.ContainsKey ("status")) {
			status = (BaseParameterStatus)paramTable ["status"];
		}

		if (status == null)
			return;


		//calculate dmg and destroy if necessary
		if (status != null && status.crntHp <= 0) {
			Debug.Log ("destoyed!: " + status.SelfObj);
			GameManager.Instance.DecrementEnemyCount (status.SelfObj);

			//NPC case:
			//if the target is destroyed reset the param
			if(owner.GetComponent<EnemyAI>().charType == EnemyAI.CharacterType.NPC) {
				//add to npc
				Weapon.RegisterAsNpc((EnemyParameterStatus)status);
				//reset npc param
				((NpcParameterStatus)owner.GetComponent<EnemyAI>().status).ResetSearchEnemyOnAttack();
			}

			//if the event is set play it
			if (status.SelfObj.GetComponent<EnemyAI> () != null && status.SelfObj.GetComponent<EnemyAI> ().OnDestroyEnemyPrefab != null) {
				GameObject enemyDeadEventList = (GameObject)Instantiate (status.SelfObj.GetComponent<EnemyAI> ().OnDestroyEnemyPrefab);
				enemyDeadEventList.GetComponent<EventListObject> ().PlayEventList ();
			}

//			Destroy (status.SelfObj);
			//if the target is destroyed, reset the npc target info
			if (owner.GetComponent<EnemyAI> ().charType == EnemyAI.CharacterType.NPC) {
				GameManager.Instance.DestroyEnemy (owner, status.SelfObj);
				((NpcParameterStatus)owner.GetComponent<EnemyAI> ().status).ResetSearchEnemyOnAttack ();
			}
			//player destroy
			else {
				DestroyPlayer ();
			}
		}
		//if the player is alive and npc exists on the stage, let npc search the enemy
		else {
			GameObject npcObj = GameManager.Instance.crntNpcObj;
			if(npcObj != null) {
				EnemyAI npcEnemyAi = npcObj.GetComponent<EnemyAI>();
				NpcParameterStatus npcParam = (NpcParameterStatus)npcEnemyAi.status;
				npcParam.BeginSearchEnemyOnAttack(this);
			}
		}


		//if enemy is still alive and hit count is above limit, move opposite side
		if (status.SelfObj.GetComponent<EnemyAI> () != null && 
			status.SelfObj.GetComponent<EnemyAI> ().charType == EnemyAI.CharacterType.ENEMY) {
			status.SelfObj.GetComponent<EnemyAI> ().ForceMoveOnMultiAttak ();
		}
	}

	/**
	 * 
	 * after hp  become 0, animate death and trigger Stage_Home.
	 * 
	 */
	private void DestroyPlayer() {
		GameManager.Instance.player.GetComponent<Collider> ().enabled = false;
		GameManager.Instance.player.GetComponent<PlayerCharacter> ().playerAnimation.Play ("death");
	}
}
