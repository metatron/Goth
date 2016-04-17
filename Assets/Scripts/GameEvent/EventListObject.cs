using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventListObject : MonoBehaviour {
	public string id;

	//activate this event only if user has finished 
	//the event more than needClrNum4NxtEvnt times.
	public int needClrNum4NxtEvnt = 0;
	//play the next EventList instead of the crnt one.
	public EventListObject nextEventListObj;
	//the record will be stored on GameManager.Instance.eventDoneList
	public bool willStoreOnDoneList = true;

	public List<EventData> eventDataList = new List<EventData> ();
	private EventData crntEventData;
	private int index = 0;

	public static bool isEventOn = false;

	void OnTriggerEnter(Collider other) {
		//always the player is the trigger of the event.
		if (other.gameObject.GetComponent<PlayerCharacter> () == null) {
			return;
		}

		//Play event
		PlayEventList();
	}

	public void PlayEventList() {
		Debug.LogError ("================= Start EventList =============: " + gameObject);

		isEventOn = true;
		//make all ather enemies pause
		GameManager.Instance.SetDoNothingAllCharacters(true);

		GetComponent<BoxCollider> ().enabled = false;
		crntEventData = eventDataList [index];
		crntEventData.eventListObj = this;

		//set the reference to the crnt active eventlist.
		StageManager.crntEvent = this;

		StartCoroutine (PlayEvent());
	}

	IEnumerator PlayEvent() {
		crntEventData.InitEvent ();

		yield return new WaitForSeconds (crntEventData.totalTime);

		index++;
		//has next event
		if (index < eventDataList.Count) {
			//destroy objects
			crntEventData.DestroyEventDataObj ();
			//set the next event
			crntEventData = eventDataList [index];
			//start the event
			StartCoroutine (PlayEvent ());
		}
		//finish event
		else {
			//check for fade if it is finished.
			StartCoroutine (Wait4FaderFinish());

			//following script will be moved into the function Wait4FaderFinish.
			//as startcoroutine need to stop the following until the fade is finished.

//			Debug.LogError ("================= End  EventList =============: " + gameObject);
//
//			crntEventData.DestroyEventDataObj ();
//			isEventOn = false;
//			//make all other enemies move
//			GameManager.Instance.SetDoNothingAllCharacters(false);
//
//			//save done
//			if (willStoreOnDoneList) {
//				GameManager.Instance.eventDoneList.Add (id);
//				SaveLoadStatus.SaveUserParameters ();
//			}
//
//			//make all iTween animation off
//			iTween.Stop ();
//
//			//set reference to null.
//			StageManager.crntEvent = null;
		}
	}

	IEnumerator Wait4FaderFinish() {
		while (FaderObject.isFading) {
			Debug.LogError ("====Wait4FaderFinish...");
			yield return new WaitForSeconds (1.0f);
			Debug.LogError ("====Wait4FaderFinished!!!");
		}

		Debug.LogError ("================= End  EventList =============: " + gameObject);

		crntEventData.DestroyEventDataObj ();
		isEventOn = false;
		//make all other enemies move
		GameManager.Instance.SetDoNothingAllCharacters(false);

		//save done
		if (willStoreOnDoneList) {
			GameManager.Instance.eventDoneList.Add (id);
			SaveLoadStatus.SaveUserParameters ();
		}

		//make all iTween animation off
		iTween.Stop ();

		//set reference to null.
		StageManager.crntEvent = null;

	}
}
