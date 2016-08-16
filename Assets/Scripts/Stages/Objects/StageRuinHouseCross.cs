using UnityEngine;
using System.Collections;

public class StageRuinHouseCross : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<PlayerCharacter> () != null) {
			GetComponent<BoxCollider> ().enabled = false;

			iTween.ValueTo(gameObject, 
				iTween.Hash(
					"from", 0f, 
					"to", 180f, 
					"time", 5f, 
					"onupdate", "UpdateRotateValue",
					"oncomplete", "OnCompleteRotate",
					"oncompletetarget", gameObject)
			);
		}
	}

	private void UpdateRotateValue(float delta) {
		transform.localRotation = Quaternion.Euler (0.0f, 0.0f, delta);
	}

	private void OnCompleteRotate() {
	}
}
