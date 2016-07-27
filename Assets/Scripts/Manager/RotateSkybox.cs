using UnityEngine;
using System.Collections;

public class RotateSkybox : MonoBehaviour {
	private float rotateVal = 0.0f;

	void Update () {
		rotateVal += Time.deltaTime;
		transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, rotateVal, 0.0f));
	}
}
