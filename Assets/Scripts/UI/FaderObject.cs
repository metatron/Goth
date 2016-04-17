using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FaderObject : MonoBehaviour {

	public delegate void CompleteFadingDelegate();

	private CompleteFadingDelegate CompleteFadingFunc;

	private float totalSecFading;

	//necessary for eventList not to reset iTween before fade in/out complete.
	public static bool isFading = false;

	public void StargFading(float totalSecFading, CompleteFadingDelegate CompleteFadingFunc) {
		this.totalSecFading = totalSecFading;
		this.CompleteFadingFunc = CompleteFadingFunc;

		//do not allow key input
		EventListObject.isEventOn = true;

		isFading = true;

		//Start fading
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 0f,
			"to", 1f,
			"time", totalSecFading/2f,
			"onupdatetarget", gameObject,
			"onupdate", "OnUpdateFading",
			"oncompletetarget", gameObject,
			"oncomplete", "OnCompleteFading"
		));
	}

	private void OnUpdateFading(float value) {
		GetComponent<Image> ().color = new Color (0, 0, 0, value);
	}


	private void OnCompleteFading() {
		Debug.LogError ("======= OnCompleteFading ========");
		CompleteFadingFunc ();

		FinishFading ();
	}

	private void FinishFading() {
		Debug.LogError ("======= FinishFading ========");
		//Start fading
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 1f,
			"to", 0f,
			"time", totalSecFading/2f,
			"onupdatetarget", gameObject,
			"onupdate", "OnUpdateFading",
			"oncompletetarget", gameObject,
			"oncomplete", "OnCompleteFadeIn"
		));
	}

	private void OnCompleteFadeIn() {
		Debug.LogError ("======= OnCompleteFadeIn ========");

		//after fade in finished, allow input
		EventListObject.isEventOn = false;

		isFading = false;

		gameObject.SetActive (false);
	}
}
