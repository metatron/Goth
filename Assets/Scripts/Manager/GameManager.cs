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
		ResetPlayerParam();

		//after load status, remove collected items
		crntStageData.InitStageCollectionItems();
	}

	public void Update() {
		Check4SlowMo ();
	}

	public void ResetPlayerParam() {
		if (player == null) {
			return;
		}

		playerParam.crntHp = playerParam.GetBaseHp();
		playerParam.crntAtk = playerParam.GetBaseAtk();
		playerParam.crntAtkSpeed = playerParam.GetBaseAtkSpeed();
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
		crntVisibleDistance = ghostSelf.maxSpotDistance + ghostSelf.status.crntVisibleInc;

		//test output
		if (ghostSelf.status.GetType () == typeof(NpcParameterStatus)) {
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
	 * (The other Enemy status init function is: SummonEnemyByStatus. Called from EventData)
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
		if (enemyObj.GetComponent<EnemyAI> ().status.type == BaseParameterStatus.GhostType.TiedGhost) {
			tmpPos.y += 300.0f;
		}

		enemyObj.position = tmpPos;


		//if enemy is close contact attacker, init weapon
		if (enemyObj.GetComponent<EnemyAI> ().enemyMotion.closeWeaponPrefab != null) {
			EnemyAI enemyAi = enemyObj.GetComponent<EnemyAI> ();
			InitCloseContactWeapon (ref enemyAi);
		}
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
		GameObject npcPrefab = npcParam.GetPrefab();
		string npcName = npcPrefab.name;
		GameObject npcObject = (GameObject)Instantiate(npcPrefab);

		npcObject.name = npcName + "_NPC";
		crntNpcObj = npcObject; //register npc

		EnemyAI enemyAI = npcObject.GetComponent<EnemyAI> ();
		enemyAI.charType = EnemyAI.CharacterType.NPC;

		//temporary set to off screen (need to set before WaitForEndOfFrame)
		npcObject.transform.position = new Vector3(0.0f, 10000.0f, 0.0f); //player.transform.position;

		//need this for setting enemyMotion.targetPos = null after Instantiate.
		yield return new WaitForEndOfFrame ();
		
		//delete Enemy replated status
		enemyAI.target = null;
		enemyAI.enemyMotion.targetPos = null;

		//initialize NPC status
		enemyAI.status = null;
		enemyAI.status = npcParam;
		enemyAI.status.SelfObj = npcObject;
		//if this is true on summon, it wont follow the player.
		((NpcParameterStatus)enemyAI.status).isSearchingOnAttack = false;

		//calculate the base parameters such as HP, ATK, MoveSpd, AtkSpd, Visibility.
		enemyAI.status.InitCharacterParameters ();
		npcParam.PrintParam ();

		//init enemy weapon
		enemyAI.enemyMotion.InitEnemyMotion ();
		//if enemy is close contact attacker, init weapon
		if (enemyAI.enemyMotion.closeWeaponPrefab != null) {
			InitCloseContactWeapon (ref enemyAI);
		}

		//do not follow until summoning finished
		enemyAI.doNothing = true;

		//initialize summon particle
		Vector3 summonParticlePos = player.transform.position;
		summonParticlePos.y = player.transform.position.y + 300;
		//looking right
		summonParticlePos.x = player.transform.position.x + 300;
		//looking left
		if (playerParam.GetCharacterDirection () == BaseParameterStatus.CharacterDirection.LEFT) {
			summonParticlePos.x = player.transform.position.x - 300;
		}
		GameObject summonParticleObj = Instantiate ((GameObject)Resources.Load ("Prefabs/Particle/SummonParticle"));
		summonParticleObj.transform.position = summonParticlePos;


		//set npcObject as passing params
		Hashtable paramTable = new Hashtable ();
		paramTable.Add ("npcObject", npcObject);

		//set the npc same position as particle
		npcObject.transform.localScale = Vector3.zero;
		//get the y position from FixedPosVec
		float ypos = npcObject.GetComponent<FollowScript>().fixedPos.y;
		npcObject.transform.position = new Vector3(summonParticlePos.x, ypos, 0.0f);
		iTween.ScaleTo(npcObject, 
			iTween.Hash(
				"scale", enemyAI.defaultSizeVec,
				"time", 3.0f,
				"islocal", true,
				"oncompletetarget", gameObject,
				"oncomplete", "OnNpcSummonFinished",
				"oncompleteparams", paramTable
			)
		);

		//delete from the enemylist
		totalEnemyList.Remove (npcObject);
	}

	private void OnNpcSummonFinished(object paramObject) {
		Hashtable paramTable = (Hashtable)paramObject;
		GameObject npcObject = null;
		if (!paramTable.ContainsKey ("npcObject")) {
			Debug.LogError ("Summoing NPC failed!!");
			return;
		}


		npcObject = (GameObject)paramTable ["npcObject"];
		npcObject.GetComponent<EnemyAI> ().doNothing = false;

		//enable follow script
		FollowScript followScript = npcObject.GetComponent<FollowScript> ();
		followScript.enabled = true;
		followScript.target = player;

		//set follower
		player.GetComponent<PlayerCharacter> ().npcObject = npcObject;

		//set the direction
		DecideNpcPosition ();

		//delete from the enemylist
		totalEnemyList.Remove (npcObject);
	}

	/**
	 * 
	 * Called from EventData.
	 * 
	 * enemyStatus will be directly inserted into EnemyAI.status.
	 * No increase from the LEVEL.
	 * 
	 * (The other Enemy status updater is: UpdateSummonedEnemyParam. Called from EventData)
	 * 
	 */
	public GameObject SummonEnemyByStatus(EnemyParameterStatus enemyStatus) {
		GameObject enemyPrefab = enemyStatus.GetPrefab();
		string enemyName = enemyPrefab.name;
		GameObject targetObject = (GameObject)Instantiate(enemyPrefab);
		targetObject.name = targetObject.name.Replace ("(Clone)", "");

		//delete Enemy replaced status
		EnemyAI enemyAI = targetObject.GetComponent<EnemyAI> ();
		enemyAI.target = null;
		enemyAI.enemyMotion.targetPos = null;

		//initialize NPC status
		enemyAI.status = enemyStatus;
		enemyAI.status.SelfObj = targetObject;
		enemyAI.charType = EnemyAI.CharacterType.ENEMY;

//		enemyAI.status.InitCharacterParameters();
		enemyAI.status.PrintParam ();

		//if enemy is close contact attacker, init weapon
		if (enemyAI.enemyMotion.closeWeaponPrefab != null) {
			InitCloseContactWeapon (ref enemyAI);
		}

		return targetObject;
	}

	/**
	 * 
	 * if the ghost uses close contact weapon, need to reposition the weapon
	 * also, if initializing Enemy, weapon need to be registered in GameManager.Instance.enemyWeaponList.
	 * (Npc wont be registered since weapon will be destroyed on loading staging)
	 * 
	 * this function will be called from 
	 * SummonEnemyByStatus:      Enemy from EventData
	 * UpdateSummonedEnemyParam: Enemy from Stage Spawner
	 * SummonNpc:                Npc by summoning
	 * 
	 */
	private void InitCloseContactWeapon(ref EnemyAI enemyAI) { //ref: 初期化必須
		//if it is nurse load weapon
		if (enemyAI != null && enemyAI.status.type == BaseParameterStatus.GhostType.Nurse) {
			enemyAI.enemyMotion.InitCloseContactWeapon(new Vector3(0.0f, 0.0f, 8.0f));
		}
		else if (enemyAI != null && enemyAI.status.type == BaseParameterStatus.GhostType.WillOWisp) {
			enemyAI.enemyMotion.InitCloseContactWeapon(new Vector3(0.0f, 0.0f, 0.0f));
		}
	}

	public void DecideNpcPosition() {
		if (player == null || crntNpcObj == null) {
			return ;
		}

		BaseParameterStatus baseParam = crntNpcObj.GetComponent<EnemyAI> ().status;

		if (crntNpcObj.GetComponent<EnemyAI> ().charType != EnemyAI.CharacterType.NPC || 
			crntNpcObj.GetComponent<EnemyAI> ().doNothing) {
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

		NpcParameterStatus npcStatus = (NpcParameterStatus)npcObject.GetComponent<EnemyAI> ().status;

		GameObject foundEnemyObject = null;
		float minDistance2Enemy = float.MaxValue;
		foreach (GameObject enemy in totalEnemyList) {
			float distance = Mathf.Abs(enemy.transform.position.x - npcObject.transform.position.x);
			BaseParameterStatus.CharacterDirection enemyLeftRight = BaseParameterStatus.CharacterDirection.LEFT;
			if(npcObject.transform.position.x - enemy.transform.position.x < 0) {
				enemyLeftRight = BaseParameterStatus.CharacterDirection.RIGHT;
			}

			//if too close, target the enemy
			float minDist = Mathf.Max(100.0f, npcObject.GetComponent<EnemyAI> ().maxAttackDistance/3);
			if(distance <= minDist) {
				Debug.Log ("**********Too close enemy ("+(npcObject.GetComponent<EnemyAI> ().maxAttackDistance/3)+"): " + distance);
				return enemy.gameObject;
			}

			//looking left
			if (playerCharacter.status.GetCharacterDirection () == BaseParameterStatus.CharacterDirection.LEFT) {
				if(distance <= (npcObject.GetComponent<EnemyAI> ().maxAttackDistance + npcObject.GetComponent<EnemyAI> ().status.crntVisibleInc) && 
					enemyLeftRight == BaseParameterStatus.CharacterDirection.LEFT) {
					if(distance < minDistance2Enemy) {
						foundEnemyObject = enemy.gameObject;
						minDistance2Enemy = distance;
					}
				}
			}
			//looking right
			else {
				if(distance <= (npcObject.GetComponent<EnemyAI> ().maxAttackDistance + npcObject.GetComponent<EnemyAI> ().status.crntVisibleInc) && 
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
						((NpcParameterStatus)crntNpcObj.GetComponent<EnemyAI>().status).BeginSearchEnemyOnAttack(enemyWeapon);
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

	/**
	 * 
	 * this will play the destroy process of the EnemyObject.
	 * called from Weapon.AfterDamageProcess and EnemyWeapon.AfterDamageProcess.
	 * 
	 * 
	 */
	public void DestroyEnemy(GameObject attackerObject, GameObject enemyObject) {
		//init particle
		GameObject particle = (GameObject)Instantiate(Resources.Load("Prefabs/Particle/DeathParticle"));
		particle.transform.parent = enemyObject.transform;
		particle.transform.localPosition = Vector3.zero;
		particle.transform.parent = null;

		//destroy enemy
		Hashtable paramTable = new Hashtable ();
		paramTable.Add ("enemyObject", enemyObject);
		paramTable.Add ("attackerObject", attackerObject);

		iTween.ScaleTo(enemyObject, 
			iTween.Hash(
				"scale", Vector3.zero,
				"time", 0.5f,
				"islocal", true,
				"oncompletetarget", gameObject,
				"oncomplete", "OnDestroyEnemyFinished",
				"oncompleteparams", paramTable
			)
		);
	}

	private void OnDestroyEnemyFinished(object paramObject) {
		Hashtable paramTable = (Hashtable)paramObject;
		GameObject enemyObject = null;
		if (!paramTable.ContainsKey ("enemyObject")) {
			Debug.LogError ("Destroy Enemy failed!!");
			return;
		}
		enemyObject = (GameObject)paramTable ["enemyObject"];

		Destroy (enemyObject);

		//get attacker and if it exists, turn off the isSearchingOnAttack
		if (!paramTable.ContainsKey ("attackerObject")) {
			Debug.LogError ("attackerObject not exists!!");
			return;
		}

		GameObject attackerObject = (GameObject)paramTable ["attackerObject"];

		//turn tis off or otherwise the npc will not follow the player. (if EnemyAI is null, could be player character)
		if (attackerObject.GetComponent<EnemyAI> () != null) {
			((NpcParameterStatus)attackerObject.GetComponent<EnemyAI> ().status).isSearchingOnAttack = false;
		}
	}
}
