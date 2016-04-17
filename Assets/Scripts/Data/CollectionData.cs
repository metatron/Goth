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

	public Vector3 resizeVec = Vector3.one;

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
