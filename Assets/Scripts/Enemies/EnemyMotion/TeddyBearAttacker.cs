using UnityEngine;
using System.Collections;
using SmoothMoves;

public class TeddyBearAttacker : EnemyMotionInterface {
	private EnemyAI enemyAI;
	private BoneAnimation boneAnim;

	private Vector3 backupPos = Vector3.zero;

	void Awake() {
		enemyAI = transform.GetComponent<EnemyAI> ();
		enemyAI.enemyMotion = transform.GetComponent<EnemyMotionInterface> ();
		boneAnim = enemyAI.GetComponent<BoneAnimation>();
		
		boneAnim.RegisterUserTriggerDelegate (OnUserTriggerEvent);



	}


	override public IEnumerator DoAttack() {
		if (GetCollider () != null) {
			GetCollider ().enabled = true;
		}
		boneAnim.Play ("attack");
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

		//after attack animation ends
		if (triggerEvent.tag == "attackafter") {
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
