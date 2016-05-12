using UnityEngine;
using System.Collections;

public class GhostMotion : MonoBehaviour {
	private const int MAX_RANGE = 1000;

	void Start () {
		//left edge of screen = camera.x + (screen.width/2) + 300
		Vector3 Vec1 = new Vector3(Camera.main.transform.position.x + (Screen.width/2) + 300, 0.0f, -10.0f);
		//right edge of screen = camera.x - (screen.width/2) - 300
		Vector3 Vec2 = new Vector3(Camera.main.transform.position.x - (Screen.width/2) - 300, 0.0f, -10.0f);

		Vector3 startVec = Vector3.zero;
		Vector3 endVec = Vector3.zero;

		//ghost can move either right to left, left to right
		int rnd = Random.Range (0, MAX_RANGE);
		if (rnd < MAX_RANGE / 2) {
			startVec = Vec2;
			endVec = Vec1;
		} else {
			startVec = Vec1;
			endVec = Vec2;
		}

		//set to start pos
		transform.position = startVec;

		Debug.LogError ("**************** startVec: " + startVec + ", endVec: " + endVec);


		iTween.MoveTo (gameObject, iTween.Hash (
			"position", endVec,
			"islocal", false,
			"time", 1.0f,
			"oncompletetarget", gameObject,
			"oncomplete", "DestroySelf"
		));
	}

	private void DestroySelf() {
		Destroy (gameObject);
	}
}
