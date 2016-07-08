using UnityEngine;
using System.Collections;
using SmoothMoves;

public class Weapon : MonoBehaviour {
	private GameObject player;
	private BoneAnimation playerAnimation;

	private GameObject hitParticleBloodPrefab;
	private GameObject hitParticleSparkPrefab;

	public int attack;

	public void Start() {

		player = GameObject.FindGameObjectWithTag ("Player");
		playerAnimation = player.GetComponent<BoneAnimation> ();

	}

	void OnTriggerEnter(Collider other) {
		if (playerAnimation == null) {
			return;
		}

		bool isAttacking = playerAnimation.IsPlaying ("throw");
		if (!isAttacking) {
			return;
		}

		//if weapon hits something shake camera
		//(avoid double shake)
		if (Camera.main.GetComponent<iTween> () == null) {
			iTween.ShakePosition (Camera.main.gameObject,
				iTween.Hash (
					"amount", new Vector3(10f, 10f, 0f),
					"islocal", false,
					"time", 0.2f
				));
		}

		/** Hitting EnemyBullet **/
		EnemyWeapon enemyWpnObj = other.gameObject.GetComponent<EnemyWeapon> ();
		if (enemyWpnObj != null &&
			enemyWpnObj.owner.GetComponent<EnemyAI>().charType == EnemyAI.CharacterType.ENEMY && 
		    	(
					enemyWpnObj.owner.GetComponent<EnemyAI>().enemyMotion.GetType () == typeof(SpiritBallSpitter) ||
					enemyWpnObj.owner.GetComponent<EnemyAI>().enemyMotion.GetType () == typeof(QueenSpitter)
				)
		    ) {
			Debug.Log ("Hitting enemy bullet with: " + other.gameObject);

			enemyWpnObj.GetComponent<Collider>().enabled = false;
			if (hitParticleSparkPrefab == null) {
				hitParticleSparkPrefab = Resources.Load ("Prefabs/Battle/Effect/particle") as GameObject;
			}
			GameObject hitParticleObj1 = (GameObject) Instantiate (hitParticleSparkPrefab);
			hitParticleObj1.transform.position = other.ClosestPointOnBounds(enemyWpnObj.transform.position);

			return ;
		}

		/** Hitting Enemy **/
		if (other != null && 
		    other.gameObject != null && 
		    other.gameObject.GetComponent<EnemyAI> () != null && 
			other.gameObject.GetComponent<EnemyAI> ().charType == EnemyAI.CharacterType.NPC) {
			return ;
		}

		Debug.Log ("Hit with: " + other.gameObject + "isAttacking: " + isAttacking);
		//could be hitting on the enemy weapon.
		if (other.gameObject.GetComponent<EnemyAI> () == null) {
			return;
		}
		EnemyParameterStatus enemyStatus = (EnemyParameterStatus)(other.gameObject.GetComponent<EnemyAI>()).status;

		BoneAnimation enemyAnim = other.gameObject.GetComponent<BoneAnimation> ();

		//Play damage animation
		if (enemyAnim != null && !enemyAnim.IsPlaying ("damage")) {
			enemyAnim.Stop("attack");
			enemyAnim.Play ("damage");
			enemyAnim.PlayQueued("normal");
		}

		//generate particle
		if (hitParticleBloodPrefab == null) {
			hitParticleBloodPrefab = Resources.Load ("Prefabs/Battle/Effect/BloodEmitter") as GameObject;
		}
		GameObject hitParticleObj = (GameObject) Instantiate (hitParticleBloodPrefab);
		hitParticleObj.transform.parent = transform;
		hitParticleObj.transform.localPosition = GetComponent<BoxCollider> ().center;
		hitParticleObj.transform.parent = null;

		//disable collider (and rigidbody)
//		GetComponent<Collider> ().enabled = false;


		Hashtable paramTable = new Hashtable ();
		paramTable.Add ("status", enemyStatus);
		GameManager.PushedByDamage (gameObject.transform, other.gameObject.transform, gameObject, "AfterDamageProcess", paramTable);


//		enemyStatus.crntHp -= attack;
//
//		Debug.Log (gameObject + " damaged enemy, " + other.gameObject + ", w/ attack: " + attack + ". result HP: " + enemyStatus.crntHp);
//
//		if (enemyStatus.crntHp <= 0) {
//			RegisterAsNpc(other.gameObject.GetComponent<EnemyAI>().status);
//			GameManager.Instance.DecrementEnemyCount(other.gameObject);
//			Destroy (other.gameObject);
//		}

	}

	private void AfterDamageProcess(object paramObject) {
		if (paramObject == null)
			return;
		
		//getting status param from iTween hash.
		Hashtable paramTable = (Hashtable)paramObject;
		BaseParameterStatus status = null;
		if (paramTable.ContainsKey ("status")) {
			status = (BaseParameterStatus)paramTable ["status"];
		}
		
		if (status == null)
			return;

		status.crntHp -= attack;
		
		Debug.Log (gameObject + " damaged enemy, " + status.SelfObj + ", w/ attack: " + attack + ". result HP: " + status.crntHp);
		
		if (status.crntHp <= 0) {
			RegisterAsNpc((EnemyParameterStatus)status);
			GameManager.Instance.DecrementEnemyCount (status.SelfObj);
//			Destroy (status.SelfObj);
			GameManager.Instance.DestroyEnemy (status.SelfObj);

			//if the event is set play it
			if (status.SelfObj.GetComponent<EnemyAI> () != null && status.SelfObj.GetComponent<EnemyAI> ().OnDestroyEnemyPrefab != null) {
				GameObject enemyDeadEventList = (GameObject)Instantiate (status.SelfObj.GetComponent<EnemyAI> ().OnDestroyEnemyPrefab);
				enemyDeadEventList.GetComponent<EventListObject> ().PlayEventList ();
			}
		} 
		//if the enemy is not yet dead and hit count is over move it to opposite side
		else {
			status.SelfObj.GetComponent<EnemyAI> ().ForceMoveOnMultiAttak ();
		}

	}



	/**
	 * 
	 * Set the DEFEATED enemy parameter. not the default enemy param.
	 * need divergence.
	 * 
	 */
	public static void RegisterAsNpc(EnemyParameterStatus targetParam) {
		if (targetParam.friendPossibility <= 0) {
			return ;
		}

		//calculate the possibility
		int rnd = Random.Range(0,101); //max exclusive
		if (rnd > 0 && rnd <= targetParam.friendPossibility) {
			Debug.LogError("RegisterAsNpc [Succeeded] friendPossibility: " + targetParam.friendPossibility + ", rnd: " + rnd);

			//get the Prefab parameter
			//the npc's parameter is based on basic ghost values.
			//the enemy parameters are differed for each stages.
			GameObject enemyPrefab = targetParam.SelfObj.GetComponent<EnemyAI>().status.GetPrefab();
			BaseParameterStatus copyingStatus = enemyPrefab.GetComponent<EnemyAI> ().status;

			//Convert EnemyParam to NpcParam
			NpcParameterStatus npcParameter = new NpcParameterStatus ();

			npcParameter.minHp = copyingStatus.minHp;
			npcParameter.maxHp = copyingStatus.maxHp;

			npcParameter.minHp = copyingStatus.minHp;
			npcParameter.maxAtk = copyingStatus.maxAtk;

			npcParameter.minMoveSpeed = copyingStatus.minMoveSpeed;
			npcParameter.maxMoveSpeed = copyingStatus.maxMoveSpeed;

			npcParameter.minAtkSpeed = copyingStatus.minAtkSpeed;
			npcParameter.maxAtkSpeed = copyingStatus.maxAtkSpeed;

			npcParameter.minVisibleInc = copyingStatus.minVisibleInc;
			npcParameter.maxVisibleInc = copyingStatus.maxVisibleInc;

			npcParameter.level = 1;
			npcParameter.type = copyingStatus.type;
			npcParameter.pattern = copyingStatus.pattern;
			npcParameter.rarity = copyingStatus.rarity;

			//copy skills
			npcParameter.skillPathList = copyingStatus.skillPathList;

			GameManager.Instance.SetNpc (npcParameter);

			SaveLoadStatus.SaveUserParameters ();

			return;
		}

		Debug.LogError("RegisterAsNpc [Failed] friendPossibility: " + targetParam.friendPossibility + ", rnd: " + rnd);
	}

}
