using UnityEngine;
using System.Collections;

public class HangRope : MonoBehaviour {
	public float rotation = 5.0f;
	public float multiplier = 10.0f;

	private float crntRotation = 0.0f;
	private int direction = 0; //0: to right, 1: to left

	// Use this for initialization
	void Start () {
	}

	void Update() {
		crntRotation += multiplier*Time.deltaTime;

		if (crntRotation >= rotation && direction == 0) {
			direction = 1;
			multiplier *= -1;
		} else if (crntRotation <= -rotation && direction == 1) {
			direction = 0;
			multiplier *= -1;
		}
			
		transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, crntRotation));
	}

}
