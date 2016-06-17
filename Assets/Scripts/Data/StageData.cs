using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageData : MonoBehaviour {
	[SerializeField]
	public string title;
	[SerializeField]
	public int level;
	[SerializeField]
	public string prefabPath;

	public List<Spawner> spawnerList = new List<Spawner> ();

	public List<EnemyParameterStatus> enemyStatusList = new List<EnemyParameterStatus>();

	public List<CollectionData> collectionItemList = new List<CollectionData>();

	public List<EventListObject> stageEventList = new List<EventListObject> ();

	public GameObject homeDoorObj; //only home stage has this object.

	public float minStageLight = 0.2f;
	public float maxStageLight = 0.5f;


	void Start() {
		ShakeEffect.ALPHA_MIN = minStageLight;
		ShakeEffect.ALPHA_MAX = maxStageLight;
	}

	public void InitStageCollectionItems() {
		//check if player already have the collection item.
		//if player has it, delete it from the stage.
		foreach (string collectionId in GameManager.Instance.playerCollectedItemList) {
			for (int i=0; i<collectionItemList.Count; i++) {
				if (collectionId == collectionItemList [i].id) {
					Destroy (collectionItemList [i].gameObject);
					collectionItemList.RemoveAt (i);
				}
			}
		}

//		List<string> checkedDoneEventIdList = new List<string>();
		//check if player already experienced the event on the list.
		//if so, remove it from the stage
		for (int i = 0; i < stageEventList.Count; i++) {
			foreach (string doneEventId in GameManager.Instance.eventDoneList) {
				//if the eventId has been checked, skip it.
//				if (checkedDoneEventIdList.Contains (doneEventId)) {
//					continue;
//				}

				//player has Exped the event.
				if (stageEventList [i].id == doneEventId) {

					//if nextEvent is not set, remove it from the list
					if (stageEventList [i].nextEventListObj == null) {
						Destroy(stageEventList [i].gameObject);
					}
					//if theres number set, compare with the exped num of the event
					else if (stageEventList [i].needClrNum4NxtEvnt >= GameManager.Instance.GetNumOfDoneEvent (doneEventId)) {
						//replace it
						GameObject nextEventObj = (GameObject)Instantiate(stageEventList [i].nextEventListObj.gameObject);
						Vector3 pos = new Vector3(stageEventList [i].transform.position.x, stageEventList [i].transform.position.y, stageEventList [i].transform.position.z);
						nextEventObj.transform.position = pos;
						nextEventObj.transform.SetParent (transform);

						//destroy prev object
						Destroy(stageEventList [i].gameObject);

						stageEventList [i] = nextEventObj.GetComponent<EventListObject>();

					}
					//store eventId into checkedList.
//					checkedDoneEventIdList.Add (doneEventId);
				}
			}
		}
	}

	/**
	 * 
	 * loads enemy prefabs.
	 * this function is used at Spawner.cs.
	 * 
	 * 
	 */
	public GameObject GetEnemyPrefab(Spawner spawner, out EnemyParameterStatus chosenEnemy) {
		//get the list of enemystatus that can be instantiated.
		List<EnemyParameterStatus> possibleEnemyList = new List<EnemyParameterStatus> ();
		foreach (EnemyParameterStatus enemyStatus in enemyStatusList) {
			if(enemyStatus.spawner == null || enemyStatus.spawner.Equals(spawner)) {
				possibleEnemyList.Add(enemyStatus);
			}
		}

		//check if theres at lease one enemy
		if (possibleEnemyList.Count <= 0) {
			throw new UnityException("No enemy status defined in the Stage: " + title);
		}

		//get the enemy type
		int rnd = Random.Range (0, possibleEnemyList.Count);
		chosenEnemy = possibleEnemyList [rnd];

		//get Prefab (init enemy param status)
		return chosenEnemy.GetPrefab ();
//		enemyPrefab.GetComponent<EnemyAI>().CopyEnemyStatus(chosenEnemy);
	}
}
