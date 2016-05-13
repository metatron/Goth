using UnityEngine;
using System.Collections;
using SmoothMoves;

public class GhostMotion : MonoBehaviour {
	private const int MAX_RANGE = 1000;
	private const int X_START_POS = 1000;

	void Start () {
		//left edge of screen = camera.x + (screen.width/2) + 300
		Vector3 Vec1 = new Vector3(Camera.main.transform.position.x + (Screen.width/2) + 1000, 0.0f, -10.0f);
		//right edge of screen = camera.x - (screen.width/2) - 300
		Vector3 Vec2 = new Vector3(Camera.main.transform.position.x - (Screen.width/2) - 1000, 0.0f, -10.0f);

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


		//alpha
		iTween.ValueTo(gameObject, iTween.Hash(
				"from", 1.0f, 
				"to", 0f, 
				"time", 2.0f, 
				"onupdate", "UpdateValue"
		));

		//move
		iTween.MoveTo (gameObject, iTween.Hash (
			"position", endVec,
			"islocal", false,
			"time", 5.0f,
			"oncompletetarget", gameObject,
			"oncomplete", "DestroySelf"
		));
	}

	private void DestroySelf() {
		Debug.LogError ("*********1");
		Destroy (gameObject);
	}

	private void UpdateValue(float alpha) {
		GetComponent<SmoothMoves.Sprite> ().color.a = alpha;
	}
}
