using UnityEngine;
using System.Collections;

public class StageRuinHouseDarkness : MonoBehaviour {

	void Start () {
		StartCoroutine (DarknessAction ());
	}

	private IEnumerator DarknessAction() {
		//wait for while and enlarge
		yield return new WaitForSeconds (3.0f);

		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 1.0f, 
			"to", 5.0f, 
			"time", 0.5f, 
			"onupdate", "UpdateValue",
			"oncompletetarget", gameObject,
			"oncomplete", "OnScaleFinished"
		));
	}

	private void UpdateValue(float delta) {
		Vector3 tmpVec = transform.localScale;
		tmpVec.x = delta;
		tmpVec.y = delta;
		tmpVec.z = delta;
		transform.localScale = tmpVec;
	}

	private void OnScaleFinished() {
		Debug.LogError ("******************1");
	}
}
