using UnityEngine;
using System.Collections;

public class ParticleSelfDestroy : MonoBehaviour {
	private ParticleSystem particleObj;
	public float additionalTime = 3.0f;

	void Start () {
		particleObj = GetComponent<ParticleSystem> ();

		if (!particleObj.loop) {
			Destroy (gameObject, particleObj.duration + additionalTime);
		}
	}
}
