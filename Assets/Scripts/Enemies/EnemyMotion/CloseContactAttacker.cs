using UnityEngine;
using System.Collections;
using SmoothMoves;

public class CloseContactAttacker : EnemyMotionInterface {
	public float attackDistance = 600.0f; //distance bt the target right before getting into attack
	public float attackMoveTime = 1.0f;

	private EnemyAI enemyAI;
	private BoneAnimation boneAnim;

	private Vector3 backupPos = Vector3.zero;

	void Awake() {
		enemyAI = transform.GetComponent<EnemyAI> ();
		enemyAI.enemyMotion = transform.GetComponent<EnemyMotionInterface> ();
		boneAnim = enemyAI.GetComponent<BoneAnimation>();
		
		boneAnim.RegisterUserTriggerDelegate (OnUserTriggerEvent);



//		//if it is nurse load weapon
//		if (enemyAI != null && enemyAI.status.type == BaseParameterStatus.GhostType.Nurse) {
//			enemyAI.enemyMotion.InitCloseContactWeapon(new Vector3(0.0f, 0.0f, 8.0f));
//		}
//		else if (enemyAI != null && enemyAI.status.type == BaseParameterStatus.GhostType.WillOWisp) {
//			enemyAI.enemyMotion.InitCloseContactWeapon(new Vector3(0.0f, 0.0f, 0.0f));
//		}
	}


	override public IEnumerator DoAttack() {
		boneAnim.Play ("attackbefore");
		yield break;
	}

	override public void AttackFinished() {
		if (GetCollider () != null) {
			GetCollider ().enabled = false;
		}
		isMotionStarted = false;
	}

	public bool isAttackFinished() {
		if (boneAnim == null) {
			return false;
		}

		if (boneAnim.IsPlaying ("attack") || boneAnim.IsPlaying ("attackbefore") || boneAnim.IsPlaying ("attackafter")) {
			return false;
		}
		return true;
	}


	private void OnUserTriggerEvent(UserTriggerEvent triggerEvent) {
		//if target is destroyed durint attack for some reason, back to normal animation
		if (!IsTargetAlive ()) {
			boneAnim.Play ("normal");
			AttackFinished();
			return ;
		}

		//after attackbefore animation ends
		if (triggerEvent.tag == "attackbefore") {
			//enable weapon collider
			GetCollider ().enabled = true;

			//backup 
			backupPos = transform.position;
			//reposition
			//enemy on the right
			Vector3 reposition = transform.position; //starting position
			Vector3 moveToPos = Vector3.zero; //destination pos

			if(targetPos.position.x - transform.position.x < 0) {
				reposition.x = targetPos.position.x + attackDistance;
				moveToPos = new Vector3(targetPos.position.x + 400.0f, backupPos.y, backupPos.z);
			}
			//on left
			else {
				reposition.x = targetPos.position.x - attackDistance;
				moveToPos = new Vector3(targetPos.position.x - 400.0f, backupPos.y, backupPos.z);
			}
			transform.position = reposition;


			//use iTween to animate the position
			//if targetPos.position.y has a positive value, the character seemed to jump on attack... make it 0.
			iTween.MoveTo(
				enemyAI.gameObject,
				iTween.Hash(
				"position", moveToPos,
				"time", attackMoveTime
				));
			//play attack animation
			boneAnim.Play ("attack");
		}
		//after attack animation ends
		else if (triggerEvent.tag == "attack") {
			//use iTween to animate the position
			iTween.MoveTo(
				enemyAI.gameObject,
				iTween.Hash(
				"position", backupPos,
				"time", attackMoveTime
				));

			boneAnim.Play ("attackafter");
		}
		//after standing animation ends
		else if (triggerEvent.tag == "attackafter") {
			//play normal animation
			boneAnim.Play ("normal");

			AttackFinished();
		}
	}

	public override Collider GetCollider ()
	{
		if (crntWeaponObj != null) {
			return crntWeaponObj.GetComponentInChildren<Collider> ();
		}
		return null;
	}

	private bool IsTargetAlive() {
		if (targetPos == null) {
			return false;
		}
		return true;
	}
}
