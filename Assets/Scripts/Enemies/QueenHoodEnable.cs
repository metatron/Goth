using UnityEngine;
using System.Collections;
using SmoothMoves;

public class QueenHoodEnable : MonoBehaviour {
	[SerializeField]
	private bool enableHood = false;
	public bool EnableHood { get { return enableHood; } set {value = enableHood; } }

	void Awake() {
		if (!enableHood) {
			GetComponent<BoneAnimation> ().HideBone ("Hood", true);
			GetComponent<BoneAnimation> ().HideBone ("Mant", true);
		}
	}
}
