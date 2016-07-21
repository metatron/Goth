using UnityEngine;
using System.Collections;
using Soomla.Store;

public class ParticleAttraction : MonoBehaviour {
	public Vector3 targetPos;

	void Start() {
		Vector3[] movepath = new Vector3[3];
		movepath [0] = transform.position;
		//need to set the z position otherwise the posision wont change after conversion.
		Vector3 midPoint = new Vector3 (Random.Range (0, Screen.width), Random.Range (0, Screen.height), -Camera.main.transform.position.z);
		movepath [1] = Camera.main.ScreenToWorldPoint(midPoint);
		//Debug.LogError (midPoint + "-->" + movepath [1]);
		movepath [2] = targetPos;

		iTween.MoveTo (gameObject, iTween.Hash (
			"path", movepath,
			"orienttopath", true,
			"speed", 1000.0f,
			"easetype", iTween.EaseType.linear
		));


	}

	void OnTriggerEnter(Collider other) {
		//if hit w/ non-player, skip it
		PlayerCharacter playerChar = other.gameObject.GetComponent<PlayerCharacter> ();
		if (playerChar == null) {
			return;
		}

		//one spirit for each particle.
		StoreInventory.GiveItem (ShopItemAssets.SPIRIT_CURRENCY_ITEMID, 1);

		Destroy (gameObject);
	}
}