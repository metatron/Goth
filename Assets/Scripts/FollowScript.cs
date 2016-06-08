using UnityEngine;
using System.Collections;
using SmoothMoves;

/**
 * 
 * this class is activated when the GameObject is NPC.
 * 
 */
public class FollowScript : MonoBehaviour {
	public GameObject target;
	public Vector3 fixedPos;
	public Vector3 OffsetPos;
	public float smoothTime = 0.5f;

	public const float MOVELIMIT_VELOCITY = 20.0f;
	protected Vector3 crntVelocity;

	//npc, enemies need to be behind of player.
	//if below this value, the drawing order is messed up.
	public const float DEFAULT_Z_POS = 35.0f;


	// Update is called once per frame
	protected virtual void LateUpdate () {
		BoneAnimation boneAnim = GetComponent<BoneAnimation> ();
		if (target == null || boneAnim == null) {
			return;
		}

		//CloseContactAttacker needs to move freely
		if (boneAnim.IsPlaying ("attackbefore") || boneAnim.IsPlaying ("attack") || boneAnim.IsPlaying ("attackafter")) {
			return;
		}

		//after this line, it is not attacking moment

		EnemyAI enemyAi = GetComponent<EnemyAI> ();
		if (Mathf.Abs (crntVelocity.x) > MOVELIMIT_VELOCITY && enemyAi.AnimationExists ("walk")) {
			boneAnim.Play ("walk");
		}
		else {
			boneAnim.Play ("normal");
		}

		//if searching on attack, do not follow
		if (enemyAi.charType == EnemyAI.CharacterType.NPC) {
			if(((NpcParameterStatus)enemyAi.status).isSearchingOnAttack) {
				return ;
			}
		}

		//fixed position
		Vector3 newPos = target.transform.position;
		if (fixedPos.x != 0) {
			newPos.x = fixedPos.x;
		}
		if (fixedPos.y != 0) {
			newPos.y = fixedPos.y;
		}
		if (fixedPos.z != 0) {
			newPos.z = fixedPos.z;
		}

		//add offset
		if (OffsetPos.x != 0) {
			newPos.x += OffsetPos.x;
		}
		if (OffsetPos.y != 0) {
			newPos.y += OffsetPos.y;
		}
		if (OffsetPos.z != 0) {
			newPos.z += OffsetPos.z;
		}

		newPos.z += DEFAULT_Z_POS; //default


		transform.position = Vector3.SmoothDamp (transform.position, newPos, ref crntVelocity, smoothTime);
	}
}
