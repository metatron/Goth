using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CollectionData : MonoBehaviour {
	[SerializeField]
	public string id;
	[SerializeField]
	public string title;
	public string desc;
	[SerializeField]
	public string prefabPath;

	//resize when opening CollectionList window
	public Vector3 collectionListSizeVec = Vector3.one;
	//initial pos on CollectionList window
	public Vector3 collectionListPosVec = Vector3.zero;
	//initila rotation on collectionlist window
	public Vector3 collectionListRotVec = Vector3.zero;

	void OnTriggerEnter(Collider other) {
		//if none player hits the collection, do nothing.
		if (other.gameObject.GetComponent<PlayerCharacter> () == null) {
			return;
		}

		PlayerCharacter player = other.gameObject.GetComponent<PlayerCharacter> ();
		player.playerAnimation.Stop();
		player.playerAnimation.Play ("pickup");

		//save the item
		if (!GameManager.Instance.playerCollectedItemList.Contains (id)) {
			GameManager.Instance.playerCollectedItemList.Add (id);
			SaveLoadStatus.SaveUserParameters ();
		}

		//if player hits the collection item, play the animation
		//and at the end of the animation, popup the collection popup
		MenuManager.pickupCollectionData = this;
	}

}
