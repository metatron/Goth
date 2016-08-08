using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;



/**
 * 
 * creates Enemy Object.
 * for the Animation, it needs
 * 
 * normal
 * walk
 * attack:  needs attackafter trigger
 * damage
 * 
 * optional: (might have to create following according to the character type)
 * attackbefore
 * attackafter
 * 
 * 
 * 
 * 
 * 
 */
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (BoxCollider))]
[RequireComponent (typeof (SpawnAI))]
[RequireComponent (typeof (FollowScript))]
[RequireComponent (typeof (MeshSortingLayer))]
public class EnemyAI : MonoBehaviour {
	public enum CharacterType : int
	{
		NPC = 0,
		ENEMY = 1
	};


	public float maxAttackDistance;
	//******** will be decreased by level ****************//
	public float maxAttackWait;		//how long wait for another attack (minimum: about 3.0f ~ )
	public float cumulativeAttackWaitTime;
	public bool isFirstAttack = true;

	//******** will be increased by level ****************//
	public float maxSpotDistance = 1000.0f;	 //if the target is closer than this value enemy will start moving

//	public float maxMoveSpeed;				//how fast move
	public float maxMoveTime;				//how long move at once
	public float maxMoveWait;				//how log wait for another move

	public float cumulativeMoveTime;
	public float cumulativeMoveWaitTime;

	public Transform target;
	public float distanceTarget;

	private Vector3 lastTargetPlace; //last spotted position

	public float maxNpcDetectTargetTime = 0.2f;			//wait for npc to detect target
	public float cumulativeNpcDetectTargetTime = 0.0f;	//cumulate time



	private Transform myTransform;

	//need to detect the object is moving
	private Vector3 lastPosition = Vector3.zero;

	//default type is enemy
	public CharacterType charType = CharacterType.ENEMY;

	//required components.
	//place it manually.
	public EnemyMotionInterface enemyMotion;

	public BaseParameterStatus status = new BaseParameterStatus ();


	//Force move parameters.
	public const int MAXHITNUM_ON_FORCEMOVE = 5;	//num if hits
	public int crntNumOfMultiHit = 0; 				//num of hits. will be reset on time.
	public const float MAXTIME_ON_FORCEMOVE = 5.0f;
	public float cumulativeTimeOfMultiHit = 0.0f;	//cumulative time begins when hit >0

	//flag that make enemy do nothing.
	public bool doNothing = false;

	//default size of the enemy
	public Vector3 defaultSizeVec = Vector3.one;

	//size for Ghost list
	public Vector3 ghostListSizeVec = Vector3.one;

	//EventListObject whitch will be active if the enemy is destroyed.
	public GameObject OnDestroyEnemyPrefab;


	void Awake() {
		myTransform = transform;
		lastPosition = myTransform.position;
	}

	void Start() {
		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			target = GameObject.FindGameObjectWithTag ("Player").transform;
		} else {
			Debug.LogWarning("WARNING! GameObject.FindGameObjectWithTag (\"Player\") is null.(" + gameObject + ")");
		}

		if (enemyMotion != null) {
			//TODO update Enemy target
			enemyMotion.targetPos = target; //new Vector3(target.position.x, 0.0f, target.position.z);
//			enemyMotion.InitEnemyMotion();
		} else {
			Debug.LogWarning("WARNING! EnemyAI.enemyMosion is null.(" + gameObject + ")");
		}

		if (status != null) {
			status.SelfObj = gameObject;
		}


		//register w/ GameManager class
		GameManager.Instance.SetEnemyCharacter (gameObject);

		//update size
		gameObject.transform.localScale = defaultSizeVec;
	}

	void Update() {
		//if flag "doNothing" is on do nothing.
		if (doNothing) {
			return;
		}
			
		//if it is npc find target
//		if (npcStatus != null) {
//			GameObject targetEnemy = GameManager.Instance.GetEnemyOnNpcSite ();
//			if(targetEnemy != null) {
//				target = targetEnemy.transform;
//				enemyMotion.targetPos = target;
//			}
//		}
		UpdateNpcTarget();

		if (target == null) {
			return ;
		}


		distanceTarget = Mathf.Abs(target.position.x - myTransform.position.x); // Vector3.Distance (target.position, myTransform.position);
		//Move
		if (
			//move by distance
			(distanceTarget <= (maxSpotDistance + status.crntVisibleInc) && distanceTarget >= maxAttackDistance) ||
			//move by charlotte on attacked (NPC only)
			((charType == CharacterType.NPC && ((NpcParameterStatus)status).isSearchingOnAttack) && distanceTarget >= maxAttackDistance)
		) {

			Move();
			lastTargetPlace = target.position;
		}

		//Attack
		if (distanceTarget <= maxAttackDistance) {
			Attack();
			lastTargetPlace = target.position;
		}


		//update char motion
		IsMoving ();

		//update on attack search param on NPC 
		if (charType == CharacterType.NPC) {
			((NpcParameterStatus)status).UpdateSearchEnemyOnAttack(Time.deltaTime);
		}
	}

	private void UpdateNpcTarget() {
		cumulativeNpcDetectTargetTime += Time.deltaTime;
		if (cumulativeNpcDetectTargetTime < maxNpcDetectTargetTime) {
			return ;
		}

		//if the target is already decided and is visible, do not update the target.
		if (IsTargetVisible()) {
			return ;
		}

		//if it is npc find target
		if (charType == CharacterType.NPC) {
			//if npc is on attack searching, do not update the target of npc
			if(((NpcParameterStatus)status).isSearchingOnAttack) {
				return ;
			}

			//if the npc spotted the enemy on her site, set the target
			GameObject targetEnemy = GameManager.Instance.GetEnemyOnNpcSite ();
			//check if the player spotted the enemy
			List<GameObject> targetEnemyList = GameManager.Instance.GetEnemiesOnSite(this);
			if (targetEnemy == null && targetEnemyList.Count > 0) {
				targetEnemy = targetEnemyList [0];
			}

			if(targetEnemy != null) {
				target = targetEnemy.transform;

				//once it get the target, disable following script
				EnableDisableFollowScript(false);
			}
			else {
				target = null;

				//once it lost the target, enable following script
				EnableDisableFollowScript(true);
			}

			if (enemyMotion != null) {
				enemyMotion.targetPos = target;
			}

		}

		cumulativeNpcDetectTargetTime = 0.0f;
	}

	private bool IsTargetVisible() {
		if (target == null) {
			return false;
		}

		float distance = Mathf.Abs(target.transform.position.x - transform.position.x);
		if (distance <= (maxSpotDistance + status.crntVisibleInc)) {
			return true;
		}
		return false;
	}


	private void Spot() {

	}

	private void Move() {
		//move and wait logic
		if (cumulativeMoveTime > maxMoveTime && cumulativeMoveWaitTime < maxMoveWait) {
			cumulativeMoveWaitTime += Time.deltaTime;
			return;
		}
		//if the wait is done, move again
		else if(cumulativeMoveTime > maxMoveTime && cumulativeMoveWaitTime > maxMoveWait) {
			cumulativeMoveTime = 0;
			cumulativeMoveWaitTime = 0;
		}

		//if it is move time, move
		if (cumulativeMoveTime <= maxMoveTime) {
//			if (gameObject.name.Contains("NPC")) {
//				Debug.Log ("*********************distanceTarget: " + distanceTarget);
//			}
			//move left 
			if(target.position.x - myTransform.position.x < 0) {
				if(myTransform.localScale.x < 0) {
					myTransform.localScale = new Vector3(defaultSizeVec.x, defaultSizeVec.y, 1.0f);
				}
				myTransform.position -= myTransform.right * status.crntMoveSpeed * Time.deltaTime;
			}
			//move right
			else if(target.position.x - myTransform.position.x > 0) {
				if(myTransform.localScale.x > 0) {
					myTransform.localScale = new Vector3(-defaultSizeVec.x, defaultSizeVec.y, 1.0f);
				}
				myTransform.position += myTransform.right * status.crntMoveSpeed * Time.deltaTime;
//				if (gameObject.name.Contains("NPC")) {
//					Debug.Log ("*********************position: " + myTransform.position);
//				}
			}
		}

		cumulativeMoveTime += Time.deltaTime;
	}

	private void Attack() {
		if (enemyMotion == null) {
			return;
		}

		//for debugging
		if (charType == CharacterType.NPC) {
//			Debug.Log ("is npc attack preparation");
//			NpcParameterStatus npcStatus = (NpcParameterStatus)GetStatus();
//			npcStatus.isSearchingOnAttack = true; //necessary for npc to move freely until enemy's dead
		}

		//turn right or left
		if(target.position.x - myTransform.position.x < 0) {
			if(myTransform.localScale.x < 0) {
				myTransform.localScale = new Vector3(defaultSizeVec.x, defaultSizeVec.y, 1.0f);
			}
		}
		//move right
		else if(target.position.x - myTransform.position.x > 0) {
			if(myTransform.localScale.x > 0) {
				myTransform.localScale = new Vector3(-defaultSizeVec.x, defaultSizeVec.y, 1.0f);
			}
		}


		//wait if it is not 1st attack,
		//if the attack is finished,
		//and cumulative time is less than max wait time.
//		if (status.GetType () == typeof(NpcParameterStatus)) {
//			Debug.LogError("cmltive: " + cumulativeAttackWaitTime + ", mxAtkWait: " + maxAttackWait + ", crntAtkSpd: " + status.crntAtkSpeed + 
//				", res: [" + cumulativeAttackWaitTime + " < " + (maxAttackWait - status.crntAtkSpeed) + "]");
//		}
		float atkWaitTime = Mathf.Max(1.0f, (maxAttackWait - status.crntAtkSpeed));
		if (!isFirstAttack && !enemyMotion.isMotionStarted && cumulativeAttackWaitTime < atkWaitTime) {
			cumulativeAttackWaitTime += Time.deltaTime;
			return;
		}

		//attack animation begins
		if (!enemyMotion.isMotionStarted) {
			if (status.GetType () == typeof(NpcParameterStatus)) {
				Debug.LogError ("**********1: " + atkWaitTime);
			}
			enemyMotion.isMotionStarted = true;
			isFirstAttack = false;
			cumulativeAttackWaitTime = 0;
			StartCoroutine (enemyMotion.DoAttack ());
		}
	}

	/**
	 * 
	 * if enemy gets too many hits,
	 * force move enemy to opposite side
	 * 
	 */
	public bool ForceMoveOnMultiAttak() {
		if (GameManager.Instance.player == null) {
			return false;
		}

//		float distance = Vector3.Distance (GameManager.Instance.player.transform.position, transform.position);

		//increment the hit count (excluding 0)
		crntNumOfMultiHit++;

		if (crntNumOfMultiHit >= MAXHITNUM_ON_FORCEMOVE) {
			//enemy on right
			if (GameManager.Instance.player.transform.position.x - transform.position.x < 0) {
				transform.position = new Vector3 (transform.position.x - NpcParameterStatus.MAX_SEARCHDISTANCE_ONATTACK, transform.position.y, transform.position.z);
			}
			//enemy on left
			else {
				transform.position = new Vector3 (transform.position.x + NpcParameterStatus.MAX_SEARCHDISTANCE_ONATTACK, transform.position.y, transform.position.z);
			}

			//reset counts
			ResetForceMoveParam ();

			return true;
		} 

		return false;
	}

	/**
	 * 
	 * reset params that affect attack.
	 * called from ForceMoveOnMultiAttak
	 * 
	 */
	public void ResetForceMoveParam() {
		crntNumOfMultiHit = 0;
		cumulativeTimeOfMultiHit = 0.0f;
		enemyMotion.isMotionStarted = false;
	}


	/**
	 * 
	 * Called from Spawner.cs After UpdateSummonedEnemyParam.
	 * the parameter wont affect by the level.
	 * enemyStatus is defined at the StageData.
	 * 
	 * 
	 */
	public void CopyEnemyStatus(EnemyParameterStatus enemyStatus) {
		status = new EnemyParameterStatus ();

		status.minHp = enemyStatus.minHp;
		status.maxHp = enemyStatus.maxHp;
		status.crntHp = enemyStatus.crntHp;

		status.minAtk = enemyStatus.minAtk;
		status.maxAtk = enemyStatus.maxAtk;
		status.crntAtk = enemyStatus.crntAtk;

		status.minMoveSpeed = enemyStatus.minMoveSpeed;
		status.maxMoveSpeed = enemyStatus.maxMoveSpeed;
		status.crntMoveSpeed = enemyStatus.crntMoveSpeed;

		status.minAtkSpeed = enemyStatus.minAtkSpeed;
		status.maxAtkSpeed = enemyStatus.maxAtkSpeed;
		status.crntAtkSpeed = enemyStatus.crntAtkSpeed;

		status.minVisibleInc = enemyStatus.minVisibleInc;
		status.maxVisibleInc = enemyStatus.maxVisibleInc;

		status.level = enemyStatus.level;

		status.type = enemyStatus.type;

		((EnemyParameterStatus)status).spiritNum = enemyStatus.spiritNum;

		((EnemyParameterStatus)status).friendPossibility = enemyStatus.friendPossibility;

		((EnemyParameterStatus)status).spawner = enemyStatus.spawner;

		status.pattern = enemyStatus.pattern;
		status.rarity = enemyStatus.rarity;

		//update EnemyAI params
	}

	public bool IsMoving() {
		Vector3 displacement = transform.position - lastPosition;
		lastPosition = transform.position;
		if (Mathf.Abs(displacement.x) > 1.5f) {
			return true;
		}
		return false;
	}

	public bool AnimationExists(string name) { 
		int maxCnt = GetComponent<BoneAnimation>().mAnimationClips.Length;
		for (int i=0; i<maxCnt; i++) {
			if(GetComponent<BoneAnimation>().mAnimationClips[i].animationName == name) {
				return true;
			}
		}
		return false;
	}

	/**
	 * 
	 * used when menu is opened.
	 * 
	 * 
	 */
	public void SetAsMenuUI() {
		doNothing = true;
		enemyMotion.enabled = false;
		GetComponent<FollowScript> ().enabled = false;
	}

	public void EnableDisableFollowScript(bool enable) {
		FollowScript followScript = GetComponent<FollowScript> ();
		if (followScript != null) {
			followScript.enabled = enable;
		}
	}
}
