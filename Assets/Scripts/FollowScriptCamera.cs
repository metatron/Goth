using UnityEngine;
using System.Collections;
using SmoothMoves;

public class FollowScriptCamera : FollowScript {
	public PlayerCharacter playerCharacter;

	//default camera setting
	public const float FIELDOBVIEWMULTIPLIER = 10.0f;
	public const float DEFAULTFIELDOFVIEW = 60.0f;
	public const float MAXFIELDOFVIEW = 70.0f;
	private float crntValue = 60.0f;
	public bool isFieldViewUpdating = false;

	// Use this for initialization
	void Start () {
		GameObject target = GameObject.FindGameObjectWithTag ("Player");
		if (target != null) {
			playerCharacter = target.GetComponent<PlayerCharacter> ();
		}
	}
	
	// Update is called once per frame
	protected override void LateUpdate () {
		if (target == null || TouchDetection.Instance.isStartCharging || playerCharacter == null) {
			return;
		}
		
		//fixed position
		Vector3 newPos = target.transform.position;
		if (fixedPos.x != 0) {
			newPos.x = fixedPos.x;
		}
		if (fixedPos.y != 0) {
			newPos.y = fixedPos.y;
		}
		if (fixedPos.z != 0) {
			newPos.z = fixedPos.z;
		}
		
		//add offset
		if (OffsetPos.x != 0) {
			newPos.x += OffsetPos.x;
		}
		if (OffsetPos.y != 0) {
			newPos.y += OffsetPos.y;
		}
		if (OffsetPos.z != 0) {
			newPos.z += OffsetPos.z;
		}

		//asjust the x position according to the player's direction
		if (playerCharacter.status.GetCharacterDirection () == BaseParameterStatus.CharacterDirection.RIGHT) {
			if (isFieldViewUpdating) {
				newPos.x += 500.0f; 
			} else {
				newPos.x += 180.0f; 
			}
		} 
		else if(playerCharacter.status.GetCharacterDirection () == BaseParameterStatus.CharacterDirection.LEFT) {
			if (isFieldViewUpdating) {
				newPos.x -= 500.0f; 
			} else {
				newPos.x -= 180.0f; 
			}
		}

		UpdateFieldOfView ();
		
		transform.position = Vector3.SmoothDamp (transform.position, newPos, ref crntVelocity, smoothTime);
	}

	/**
	 * 
	 * 
	 * used to expand the main camera's field of view.
	 * use it when player is moving.
	 * 
	 * 
	 */
	public void ExpandFieldOfView() {
		isFieldViewUpdating = true;
	}

	private void UpdateFieldOfView() {
		if (isFieldViewUpdating && crntValue < MAXFIELDOFVIEW) {
			crntValue += Time.deltaTime*FIELDOBVIEWMULTIPLIER;
			Camera.main.fieldOfView = crntValue;
			if(Camera.main.fieldOfView > MAXFIELDOFVIEW) {
				Camera.main.fieldOfView = MAXFIELDOFVIEW;
			}
		} else if (!isFieldViewUpdating && crntValue > DEFAULTFIELDOFVIEW) {
			crntValue -= Time.deltaTime*FIELDOBVIEWMULTIPLIER;
			Camera.main.fieldOfView = crntValue;
			if(Camera.main.fieldOfView < DEFAULTFIELDOFVIEW) {
				Camera.main.fieldOfView = DEFAULTFIELDOFVIEW;
			}
		}
	}

	public void MinimizeFieldOfView() {
		isFieldViewUpdating = false;
	}

}
