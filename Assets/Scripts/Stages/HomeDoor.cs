using UnityEngine;
using System.Collections;

public class HomeDoor : MonoBehaviour {
	public GameObject doorObject;

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<PlayerCharacter> () != null) {
			GetComponent<BoxCollider> ().enabled = false;

			iTween.ValueTo(gameObject, 
				iTween.Hash(
					"from", 90f, 
					"to", 0f, 
					"time", 1f, 
					"onupdate", "UpdateRotateValue",
					"oncomplete", "OnCompleteRotate",
					"oncompletetarget", gameObject)
			);
		}
	}

	private void UpdateRotateValue(float delta) {
		doorObject.transform.localRotation = Quaternion.Euler (0.0f, delta, 0.0f);
	}

	private void OnCompleteRotate() {
		//open stage select menu
		MenuManager.Instance.OnSelectStageMenuButtonOpened();
	}
}
