using UnityEngine;
using System.Collections;

public class SpiritBallSpitter : EnemyMotionInterface {
	public GameObject spiritBallPrefab;

	public GameObject[] ballPositions;

	public float waitTime = 1.0f; //time to spit another spirit ball
	public float maxTime = 5.0f; //time to get to the targetPos

	public int finishedSpittingCnt = 0;

	// Use this for initialization
	void Start () {
	}
	

	override public IEnumerator DoAttack() {
		if (spiritBallPrefab == null) {
			spiritBallPrefab = Resources.Load ("Prefabs/Enemies/EnemyProps/SpiritBall") as GameObject;
		}

		finishedSpittingCnt = 0;

		for (int i=0; i<ballPositions.Length; i++) {
			
			yield return new WaitForSeconds(waitTime);


//			//TODO if the multiple hit occured and enemy moved, break the attack loop
//			if (!transform.parent.GetComponent<EnemyAI> ().enemyMotion.isMotionStarted &&) {
//				break;
//			}

			if (transform.parent.gameObject.GetComponent<EnemyAI> ().GetStatus ().GetType () == typeof(NpcParameterStatus)) {
				Debug.Log (transform.parent.gameObject + " **********1: " + waitTime + ", Time.timeScale: " + Time.timeScale);
			}

			//instantiate the ballObject
			GameObject ballObject = Instantiate (spiritBallPrefab) as GameObject;
			int rnd = Random.Range (0, ballPositions.Length); //exclusive 
			Transform tmpTransform = ballPositions[rnd].transform;

			//set destroy time
			ballObject.GetComponent<EnemyWeapon>().SetTime2Destroy(5.0f);

			//set the owner of the weapon
			ballObject.GetComponent<EnemyWeapon>().owner = transform.parent.gameObject;

			//it is npc bullet
//			if(transform.parent.GetComponent<EnemyAI>().npcStatus != null) {
//				ballObject.GetComponent<EnemyWeapon>().crntEnemyWeaponType = EnemyWeapon.EnemyWeaponType.NPC;
//			}

			//pass atk to bullet object
			BaseParameterStatus status = transform.parent.gameObject.GetComponent<EnemyAI>().GetStatus ();
			ballObject.GetComponent<EnemyWeapon>().attack = status.crntAtk;

			//set the starting pos
			ballObject.transform.position = tmpTransform.position;

			//the target is gone or changing direction.
			//break the iterator.
			if(targetPos == null) {
				break;
			}


			//add to the enemy weapon list
			GameManager.Instance.enemyWeaponList.Add(ballObject.GetComponentInChildren<EnemyWeapon>());

			Vector3 centerPos = targetPos.GetComponent<Collider>().bounds.center;
			//Vector3 aimPos = new Vector3(targetPos.position.x, targetPos.position.y+250.0f, targetPos.position.z);

			finishedSpittingCnt++;

			//tween it
			iTween.MoveTo(ballObject, iTween.Hash(
				"position", centerPos,
				"time", maxTime,
				"oncompletetarget", gameObject,
				"oncomplete", "AttackFinished")
			              );
		}
	}

	override public void AttackFinished() {
		if (finishedSpittingCnt >= ballPositions.Length) {
			isMotionStarted = false;
		}
	}

	public bool isAttackFinished() {
		if (finishedSpittingCnt >= ballPositions.Length) {
			return true;
		}
		return false;
	}
}
