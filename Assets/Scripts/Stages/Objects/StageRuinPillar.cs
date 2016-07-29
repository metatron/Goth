using UnityEngine;
using System.Collections;

public class StageRuinPillar : MonoBehaviour {

	private float rotate = 0.0f;

	void Update () {
		rotate += Time.deltaTime*2.0f;
		transform.localRotation = Quaternion.Euler (new Vector3 (0.0f, rotate, 0.0f));
	}
}
