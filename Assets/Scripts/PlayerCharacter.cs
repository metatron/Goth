using UnityEngine;
using System.Collections;
using SmoothMoves;

public class PlayerCharacter : MonoBehaviour {
	public BoneAnimation playerAnimation;
	public Transform weaponPosition;
	public GameObject currentWeapon;

	public GameObject npcObject;

	public PlayerParameterStatus status = new PlayerParameterStatus();

	// Use this for initialization
	void Start () {
		playerAnimation = GetComponent<BoneAnimation> ();
		//load weapon put it on to hand
		weaponPosition = playerAnimation.GetBoneTransform ("weapon");
		currentWeapon = (GameObject)Instantiate(Resources.Load ("Prefabs/Weapons/Knife5") as GameObject);
		currentWeapon.transform.parent = weaponPosition.transform;
		currentWeapon.transform.localPosition = new Vector3(0.0f, 0.0f, 10.0f);

		status.SelfObj = gameObject;

		playerAnimation.RegisterUserTriggerDelegate (OnCharlotteTriggerEvent);
	}

	public void InitWeaponStatus() {
		currentWeapon.GetComponentInChildren<Weapon> ().attack = status.crntAtk;
	}


	// Update is called once per frame
	void Update () {
	
	}

	private void OnCharlotteTriggerEvent(UserTriggerEvent triggerEvent) {
//		Debug.LogError ("*******" + triggerEvent.tag);
		if (triggerEvent.tag == "attackstart") {
			currentWeapon.GetComponentInChildren<Collider> ().enabled = true;
		} else if (triggerEvent.tag == "attackend" || triggerEvent.tag == "walkstart" || triggerEvent.tag == "standstart") {
			currentWeapon.GetComponentInChildren<Collider> ().enabled = false;
		} else if (triggerEvent.tag == "pickupend") {
			MenuManager.Instance.OpenCollectionPopup ();
		}
	}
	

}
