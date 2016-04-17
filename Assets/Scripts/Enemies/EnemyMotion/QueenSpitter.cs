using UnityEngine;
using System.Collections;
using SmoothMoves;

public class QueenSpitter : EnemyMotionInterface {
	public float attackDistance = 600.0f; //distance bt the target right before getting into attack
	public float attackMoveTime = 1.0f;

	public GameObject spiritBallPrefab;

	public GameObject ballPosition;

	public float waitTime = 1.0f; //time to spit another spirit ball
	public float maxTime = 5.0f; //time to get to the targetPos

	private EnemyAI enemyAI;
	private BoneAnimation boneAnim;

	void Awake() {
		enemyAI = transform.parent.GetComponent<EnemyAI> ();
		enemyAI.enemyMotion = transform.GetComponent<EnemyMotionInterface> ();
		boneAnim = enemyAI.GetComponent<BoneAnimation>();

		boneAnim.RegisterUserTriggerDelegate (OnUserTriggerEvent);

		if (spiritBallPrefab == null) {
			spiritBallPrefab = Resources.Load ("Prefabs/Enemies/EnemyProps/SpiritBall") as GameObject;
		}
	}


	override public IEnumerator DoAttack() {
		boneAnim.Play ("attack");
		yield break;
	}

	override public void AttackFinished() {
		isMotionStarted = false;
	}

	public bool isAttackFinished() {
		if (boneAnim == null) {
			return false;
		}

		if (boneAnim.IsPlaying ("attack")) {
			return false;
		}
		return true;
	}


	private void OnUserTriggerEvent(UserTriggerEvent triggerEvent) {
		//if target is destroyed during attack for some reason, back to normal animation
		if (!IsTargetAlive ()) {
			boneAnim.Play ("normal");
			AttackFinished();
			return ;
		}

		//on attack spit the ball
		if (triggerEvent.tag == "attackon") {
			//instantiate the ballObject
			GameObject ballObject = Instantiate (spiritBallPrefab) as GameObject;
			Transform tmpTransform = boneAnim.GetSpriteTransform("weapon");

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
				return ;
			}


			//add to the enemy weapon list
			GameManager.Instance.enemyWeaponList.Add(ballObject.GetComponentInChildren<EnemyWeapon>());

			Vector3 centerPos = targetPos.GetComponent<Collider>().bounds.center;
			//Vector3 aimPos = new Vector3(targetPos.position.x, targetPos.position.y+250.0f, targetPos.position.z);
			//tween it
			iTween.MoveTo(ballObject, iTween.Hash(
				"position", centerPos,
				"time", maxTime,
				"oncompletetarget", gameObject,
				"oncomplete", "AttackFinished")
			);
		}
		//after standing animation ends
		else if (triggerEvent.tag == "attackafter") {
			//play normal animation
			boneAnim.Play ("normal");

			AttackFinished();
		}
	}

	private bool IsTargetAlive() {
		if (targetPos == null) {
			return false;
		}
		return true;
	}

}
