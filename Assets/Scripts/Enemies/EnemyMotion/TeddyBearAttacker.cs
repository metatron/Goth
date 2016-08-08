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
		GetCollider ().enabled = true;
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
			//backup 
			backupPos = transform.position;
			//reposition
			//enemy on the right
			Vector3 reposition = transform.position; //starting position
			Vector3 moveToPos = Vector3.zero; //destination pos

			transform.position = reposition;


			//play attack animation
			boneAnim.Play ("attack");
		}
		//after attack animation ends
		else if (triggerEvent.tag == "attack") {
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
