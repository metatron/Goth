using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : SingletonMonoBehaviourFast<GameManager> {
	public List<GameObject> totalEnemyList = new List<GameObject> ();
	public List<GameObject> visibleEnemyList = new List<GameObject> ();
	public List<GameObject> npcVisibleEnemyList = new List<GameObject> ();

	public List<NpcParameterStatus> totalNpcList = new List<NpcParameterStatus> ();
	public GameObject crntNpcObj;

	public GameObject player;
	PlayerCharacter playerCharacter;
	public PlayerParameterStatus playerParam;

	public List<string> playerCollectedItemList = new List<string> (); //collected item ID list
	public List<string> eventDoneList = new List<string> (); //event experienced ID List; it contains multiple same names.

	public StageData crntStageData;

	//for slowmo time scale calculation
	public List<EnemyWeapon> enemyWeaponList = new List<EnemyWeapon> (); //every time the enemy weapon instantiated add to the list
	private float slowMoTimeScale; //slow motion time scale
	private float fastMoTimeScale; //fast motion time scale
	private float factor = 4f;  //factor to increase or decrease the timescale by
	private const float SLOMO_TIME = 1.0f; //how long the slomo is.


	public void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
		playerCharacter = player.GetComponent<PlayerCharacter> ();
		playerParam = player.GetComponent<PlayerCharacter> ().status;


		//TODO stage initialize
		crntStageData = StageManager.Instance.InstantiateStage("Prefabs/Stage/Stage_Home");//InitStage (playerParam.level);

		//play wakeup animation
		playerCharacter.playerAnimation.Play("wakeup");
		playerCharacter.playerAnimation.PlayQueued ("stand");

		slowMoTimeScale = Time.timeScale / factor;
		fastMoTimeScale = Time.timeScale * factor;

		SaveLoadStatus.LoadUserParameters ();

		//reset HP param
		playerParam.crntHp = playerParam.GetBaseHp();

		//after load status, remove collected items
		crntStageData.InitStageCollectionItems();
	}

	public void Update() {
		Check4SlowMo ();
	}

	/**
	 * 
	 * When Enemy is created, register w/ this function.
	 * 
	 * 
	 */
	public void SetEnemyCharacter(GameObject enemy) {
		//when instantiating for menu list, check for following flag.
		if (enemy.GetComponent<EnemyAI> ().doNothing) {
			return;
		}

		enemy.transform.parent = transform;
		totalEnemyList.Add (enemy);
	}

	/**
	 * 
	 * detect if there're enemies nearby.
	 * 
	 */
	public List<GameObject> GetEnemiesOnSite(EnemyAI ghostSelf) {
		if (playerCharacter == null || ghostSelf == null) {
			return new List<GameObject>();
		}

		float crntVisibleDistance = 1000.0f; //just in case create the default value.
		crntVisibleDistance = ghostSelf.maxSpotDistance + ghostSelf.GetStatus().crntVisibleDistance;

		if (ghostSelf.GetStatus ().GetType () == typeof(NpcParameterStatus)) {
			Debug.LogError (ghostSelf.gameObject.name + " crntVisibleDistance: " + crntVisibleDistance);
		}

		if (playerCharacter.status.GetCharacterDirection () == BaseParameterStatus.CharacterDirection.LEFT) {
			float minX = player.transform.position.x - crntVisibleDistance;
			foreach (GameObject enemyObj in totalEnemyList) {
				if (enemyObj != null && 
					enemyObj.GetComponent<EnemyAI> ().status.crntHp > 0 && 
					enemyObj.transform.position.x > minX && 
				    !visibleEnemyList.Contains (enemyObj)
				    ) { 
					visibleEnemyList.Add (enemyObj);
				} else {
					if (visibleEnemyList.Contains (enemyObj)) {
						visibleEnemyList.Remove (enemyObj);
					}
				}
			}
		} else {
			float manX = player.transform.position.x + crntVisibleDistance;
			foreach (GameObject enemyObj in totalEnemyList) {
				if (enemyObj != null && 
				    enemyObj.GetComponent<EnemyAI> ().status.crntHp > 0 && 
				    enemyObj.transform.position.x < manX && 
				    !visibleEnemyList.Contains (enemyObj)
				    ) { 
					visibleEnemyList.Add (enemyObj);
				} else {
					if (visibleEnemyList.Contains (enemyObj)) {
						visibleEnemyList.Remove (enemyObj);
					}
				}
			}
		}
		return visibleEnemyList;
	}


	/**
	 * 
	 * need to update when Enemy is destropyed.
	 * 
	 * 
	 */
	public void DecrementEnemyCount(GameObject targetObj) {

		//int length = totalEnemyList.Count;
		for (int i=0; i<totalEnemyList.Count; i++) {
			if(totalEnemyList[i].Equals(targetObj)) {
				totalEnemyList.RemoveAt(i);
			}
		}

		//length = visibleEnemyList.Count;
		for (int i=0; i<visibleEnemyList.Count; i++) {
			if(visibleEnemyList[i].Equals(targetObj)) {
				visibleEnemyList.RemoveAt(i);
			}
		}
	}

	public void DeleteAllEnemies() {
		//delete weapon list
		int cnt = enemyWeaponList.Count;
		Debug.Log ("enemyWeaponList: " + cnt);
		for (int i=0; i<cnt; i++) {
			if (enemyWeaponList[i] != null && enemyWeaponList[i].gameObject != null) {
				//if it has trail delete that first
				if (enemyWeaponList [i].trailObject != null) {
					enemyWeaponList [i].trailObject.DestroyMeshObj ();
					Destroy (enemyWeaponList [i].trailObject.gameObject);
				}
				//delete enemy weapon itself
				Destroy (enemyWeaponList [i].gameObject);
			}
		}
		enemyWeaponList.Clear ();

		//delete enemy
		cnt = totalEnemyList.Count;
		Debug.Log ("totalEnemyList: " + cnt);
		for (int i=0; i<cnt; i++) {
			if (totalEnemyList [i] != null && totalEnemyList [i].gameObject != null) {
				Destroy (totalEnemyList [i].gameObject);
			}
		}
		totalEnemyList.Clear ();

		//delete visible list
		cnt = visibleEnemyList.Count;
		Debug.Log ("visibleEnemyList: " + cnt);
		for (int i=0; i<cnt; i++) {
			if (visibleEnemyList [i] != null && visibleEnemyList [i].gameObject != null) {
				Destroy (visibleEnemyList [i].gameObject);
			}
		}
		visibleEnemyList.Clear ();
	}

	/**
	 * 
	 * 
	 * Called from Spawn.cs.
	 * set enemy init params
	 * 
	 * 
	 * 
	 */
	public void UpdateSummonedEnemyParam(Transform enemyObj) {
		if (enemyObj == null || enemyObj.GetComponent<EnemyAI> ().status == null) {
			return ;
		}

		//setting z pos
		Vector3 tmpPos = enemyObj.position;
		tmpPos.z += FollowScript.DEFAULT_Z_POS;

		//for TiedGhost, its y position needs to be fixed
		if (enemyObj.GetComponent<EnemyAI> ().GetStatus ().type == BaseParameterStatus.GhostType.TiedGhost) {
			tmpPos.y += 300.0f;
		}

		enemyObj.position = tmpPos;


		//TODO test param
//		enemyObj.GetComponent<EnemyAI> ().status.crntAtk = 10;
//		enemyObj.GetComponent<EnemyAI> ().status.maxAtk = 10;
//		enemyObj.GetComponent<EnemyAI> ().status.crntHp = 100;
//		enemyObj.GetComponent<EnemyAI> ().status.maxHp = 100;
	}

	/**
	 * 
	 * put that into NpcList.
	 * 
	 * 
	 */
	public void SetNpc(NpcParameterStatus target) {
		totalNpcList.Add (target);
//		Debug.Log ("**********SetNpc: totalNpcList count:" + totalNpcList.Count);
	}

	/**
	 * 
	 * 
	 * Summon NPC
	 * 
	 */
	public IEnumerator SummonNpc(int index) {
		if (totalNpcList.Count == 0 || crntNpcObj != null || index < 0) {
			yield break;
		}

		//instantiate Npc Object
		NpcParameterStatus npcParam = totalNpcList[index];
		npcParam.PrintParam ();
		GameObject npcPrefab = npcParam.GetPrefab();
		string npcName = npcPrefab.name;
		GameObject npcObject = (GameObject)Instantiate(npcPrefab);
		npcObject.name = npcName + "_NPC";
		crntNpcObj = npcObject; //register npc

		//need this for setting targetPos = null after Instantiate.
		yield return new WaitForEndOfFrame ();
		
		//delete Enemy replated status
		EnemyAI enemyAI = npcObject.GetComponent<EnemyAI> ();
		enemyAI.target = null;
		enemyAI.enemyMotion.targetPos = null;

		//increase spot distance (because npc is always behind the character)
		FollowScript followScpt = npcObject.GetComponent<FollowScript> ();
		// update spot distance
		//npcvisibility = maxSpotDist + additional + followOffset

		//enemyAI.maxSpotDistance the crntVisibleDist will be added on GetEnemiesOnSite.
//		enemyAI.maxSpotDistance = baseDistance + npcParam.GetBaseVisibleDistance();


		//initialize NPC status
		enemyAI.status = null;
		enemyAI.npcStatus = npcParam;
		enemyAI.npcStatus.SelfObj = npcObject;
		enemyAI.charType = EnemyAI.CharacterType.NPC;
		//if this is true on summon, it wont follow the player.
		enemyAI.npcStatus.isSearchingOnAttack = false;

		enemyAI.npcStatus.InitCharacterParameterNums ();

		//if enemy is close contact attacker, init weapon
		if (enemyAI.enemyMotion.closeWeaponPrefab != null) {
			enemyAI.enemyMotion.InitCloseContactWeapon(new Vector3(0.0f, 0.0f, 8.0f));
		}

		//enable follow script
		npcObject.transform.position = player.transform.position;
		FollowScript followScript = npcObject.GetComponent<FollowScript> ();
		followScript.enabled = true;
		followScript.target = player;
//		followScript.fixedPos = new Vector3 (0.0f, 250.0f, 0.0f);

		//set follower
		player.GetComponent<PlayerCharacter> ().npcObject = npcObject;
		DecideNpcPosition ();

		//delete from the enemylist
		totalEnemyList.Remove (npcObject);
	}

	public GameObject SummonEnemyByStatus(EnemyParameterStatus enemyStatus) {
		GameObject enemyPrefab = enemyStatus.GetPrefab();
		string enemyName = enemyPrefab.name;
		GameObject targetObject = (GameObject)Instantiate(enemyPrefab);
		targetObject.name = enemyName + "_Event";

		//delete Enemy replated status
		EnemyAI enemyAI = targetObject.GetComponent<EnemyAI> ();
		enemyAI.target = null;
		enemyAI.enemyMotion.targetPos = null;

		//initialize NPC status
		enemyAI.status = enemyStatus;
		enemyAI.npcStatus = null;
		enemyAI.charType = EnemyAI.CharacterType.ENEMY;

		enemyAI.status.InitCharacterParameterNums ();

		//if enemy is close contact attacker, init weapon
		if (enemyAI.enemyMotion.closeWeaponPrefab != null) {
			enemyAI.enemyMotion.InitCloseContactWeapon(new Vector3(0.0f, 0.0f, 8.0f));
		}

		return targetObject;
	}

	public void DecideNpcPosition() {
		if (player == null || crntNpcObj == null) {
			return ;
		}

		BaseParameterStatus baseParam = crntNpcObj.GetComponent<EnemyAI> ().GetStatus ();

		if (crntNpcObj.GetComponent<EnemyAI> ().charType != EnemyAI.CharacterType.NPC) {
			return ;
		}

		NpcParameterStatus npcParam = (NpcParameterStatus)baseParam;

		//if npc is on attacking do not follow player unless it is too far
		float distPlayerNpc = Vector3.Distance (player.transform.position, crntNpcObj.transform.position);

		if (npcParam.isSearchingOnAttack && distPlayerNpc <= NpcParameterStatus.MAX_SEARCHDISTANCE_ONATTACK) {
			return ;
		}

		FollowScript followScript = player.GetComponent<PlayerCharacter> ().npcObject.GetComponent<FollowScript> ();
		BaseParameterStatus.CharacterDirection direction = player.GetComponent<PlayerCharacter> ().status.GetCharacterDirection ();
		if (direction == BaseParameterStatus.CharacterDirection.LEFT) {
			if(followScript.OffsetPos.x < 0) {
				followScript.OffsetPos.x = Mathf.Abs(followScript.OffsetPos.x);
			}
//			followScript.OffsetPos = new Vector3 (200.0f, 250.0f, 0.0f);
			EnemyAI enemyAi = player.GetComponent<PlayerCharacter> ().npcObject.GetComponent<EnemyAI>();
			player.GetComponent<PlayerCharacter> ().npcObject.transform.localScale = new Vector3(enemyAi.defaultSizeVec.x, enemyAi.defaultSizeVec.y, 1.0f);
		} else {
			if(followScript.OffsetPos.x > 0) {
				followScript.OffsetPos.x = -followScript.OffsetPos.x;
			}
//			followScript.OffsetPos = new Vector3 (-200.0f, 250.0f, 0.0f);
			EnemyAI enemyAi = player.GetComponent<PlayerCharacter> ().npcObject.GetComponent<EnemyAI>();
			player.GetComponent<PlayerCharacter> ().npcObject.transform.localScale = new Vector3(-enemyAi.defaultSizeVec.x, enemyAi.defaultSizeVec.y, 1.0f);
		}
	}

	/**
	 * 
	 * detect if there're enemies nearby on Npc
	 * 
	 */
	public GameObject GetEnemyOnNpcSite() {
		if (player == null || totalEnemyList.Count == 0) {
			return null;
		}
		GameObject npcObject = player.GetComponent<PlayerCharacter> ().npcObject;

		if (npcObject == null) {
			return null;
		}

		NpcParameterStatus npcStatus = (NpcParameterStatus)npcObject.GetComponent<EnemyAI> ().GetStatus ();

		GameObject foundEnemyObject = null;
		float minDistance2Enemy = float.MaxValue;
		foreach (GameObject enemy in totalEnemyList) {
			float distance = Mathf.Abs(enemy.transform.position.x - npcObject.transform.position.x);
			BaseParameterStatus.CharacterDirection enemyLeftRight = BaseParameterStatus.CharacterDirection.LEFT;
			if(npcObject.transform.position.x - enemy.transform.position.x < 0) {
				enemyLeftRight = BaseParameterStatus.CharacterDirection.RIGHT;
			}

			//if too close, target the enemy
			float minDist = Mathf.Max(100.0f, npcStatus.crntVisibleDistance/3);
			if(distance <= minDist) {
				Debug.Log ("**********Too close enemy ("+(npcStatus.crntVisibleDistance/3)+"): " + distance);
				return enemy.gameObject;
			}

			//looking left
			if (playerCharacter.status.GetCharacterDirection () == BaseParameterStatus.CharacterDirection.LEFT) {
				if(distance <= npcStatus.crntVisibleDistance && 
					enemyLeftRight == BaseParameterStatus.CharacterDirection.LEFT) {
					if(distance < minDistance2Enemy) {
						foundEnemyObject = enemy.gameObject;
						minDistance2Enemy = distance;
					}
				}
			}
			//looking right
			else {
				if(distance <= npcStatus.crntVisibleDistance && 
					enemyLeftRight == BaseParameterStatus.CharacterDirection.RIGHT) {
					if(distance < minDistance2Enemy) {
						foundEnemyObject = enemy.gameObject;
						minDistance2Enemy = distance;
					}
				}
			}

			if(foundEnemyObject != null) {
				return foundEnemyObject;
			}
		}//foreach

		npcObject.GetComponent<EnemyAI> ().enemyMotion.AttackFinished ();
		return null;
	}

	/**
	 * 
	 * if the player, enemy, npc is damaged, it will backoff a bit.
	 * 
	 * 
	 */
	public static void PushedByDamage(Transform selfBodyObj, Transform dmgTarget, GameObject OnCompleteObject, string OnCompleteFunc, Hashtable paramTable = null, float time=0.1f) {
		Vector3 movingPos = dmgTarget.position;
		//hitting obj is on the right

		if(selfBodyObj.position.x - dmgTarget.position.x < 0) {
			movingPos.x += 30.0f;
		}
		//hitting obj is on the left
		else if(selfBodyObj.position.x - dmgTarget.position.x > 0) {
			movingPos.x -= 30.0f;
		}
		
		//move dmged target
		iTween.MoveTo(dmgTarget.gameObject, iTween.Hash(
			"position", movingPos,
			"time", time,
			"oncompletetarget", OnCompleteObject,
			"oncomplete", OnCompleteFunc,
			"oncompleteparams", paramTable)
		              );
	}



	/**
	 * 
	 * if EnemyWeapon (and its owner is Enemy) is nearby, add slowmo effect
	 * 
	 */
	public void Check4SlowMo() {
		if (player == null) {
			return ;
		}

		//check for enemy weapon list
		for (int i=0; i<enemyWeaponList.Count; i++) {
			EnemyWeapon enemyWeapon = enemyWeaponList[i];
			if(enemyWeapon == null || enemyWeapon.owner == null) {
				continue;
			}


			//if enemyWeapon check the distance and animating state
			EnemyAI enemyAi = enemyWeapon.owner.GetComponent<EnemyAI>();
//			Debug.Log ((enemyAi.GetStatus().GetType () != typeof(NpcParameterStatus)) + ", " + enemyAi.enemyMotion.isMotionStarted + ", " + (Vector3.Distance(player.transform.position, enemyWeapon.transform.position)));
			if(enemyAi.charType != EnemyAI.CharacterType.NPC && 
			   enemyAi.enemyMotion.isMotionStarted && 
			   Time.timeScale == 1.0f) {
				float distance = Vector3.Distance(player.transform.position, enemyWeapon.transform.position);
				if(distance < 500.0f) {
					//set the enemy on attack
					if(crntNpcObj != null) {
						((NpcParameterStatus)crntNpcObj.GetComponent<EnemyAI>().GetStatus()).BeginSearchEnemyOnAttack(enemyWeapon);
					}
					StartCoroutine(SlowMo());
					break;
				}
			}
		}
	}

	public IEnumerator SlowMo (){
		//assign new time scale value
		Time.timeScale = slowMoTimeScale;
		//reduce this to the same proportion as timescale to ensure smooth simulation
		Time.fixedDeltaTime = Time.fixedDeltaTime*Time.timeScale;

		yield return new WaitForSeconds ((1.0f/factor)*SLOMO_TIME);

		Time.timeScale = 1.0f; //default value
		Time.fixedDeltaTime = 0.02f; //default value
	}

	/**
	 * 
	 * used at Event time.
	 * 
	 * 
	 */
	public void SetDoNothingAllCharacters(bool doNothing) {
		//NPC
		if (crntNpcObj != null) {
			crntNpcObj.GetComponent<EnemyAI> ().doNothing = doNothing;
		}

		//Enemies
		foreach (GameObject enemy in totalEnemyList) {
			enemy.GetComponent<EnemyAI>().doNothing = doNothing;
		}
	}

	public int GetNumOfDoneEvent(string eventId) {
		int num = 0;
		foreach (string doneEventId in eventDoneList) {
			if (eventId.Contains (doneEventId)) {
				num++;
			}
		}
		return num;
	}
}
