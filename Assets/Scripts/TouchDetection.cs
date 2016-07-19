using UnityEngine;
using System.Collections;
using SmoothMoves;

public class TouchDetection : SingletonMonoBehaviourFast<TouchDetection> {
	public const float PLAYER_DEFAULT_SCALE = 0.89f;
	public const float DEFAULT_SCALE = 0.89f;
	public const float WEAPON_WIELD_DISTANCE = 400.0f; //

	public BoneAnimation girlAnimationObj;

	public bool isStartCharging = false;

	void Start() {
		Gesture.onTouchE += OnTouchEnter;
		Gesture.onMouse1E += OnTouchEnter;
//		Gesture.onDraggingE += OnTouchEnter;
		Gesture.onTouchUpE += OnTouchUp;
		Gesture.onMouse1UpE += OnTouchUp;

		//summoning Npc
		Gesture.onChargeStartE += OnChargingStart;
		Gesture.onChargingE += OnCharging;
		Gesture.onChargeEndE += OnChargeEnd;
	}

	void OnTouchEnter(Vector2 touchPos) {
		//if it is throwing motion, do nothing.
		if (!IsAbleToGetInput()) {
			return ;
		}

		//if dmg, do nothing
		if (PlayerParameterStatus.isHit) {
			return ;
		}

		//if menu is open
		if (MenuManager.Instance.IsMenuOpen ()) {
			return ;
		}

		if (GameManager.Instance.player == null) {
			return ;
		}

		//if charging don let the character move
		if (isStartCharging) {
			return ;
		}

		if (EventListObject.isEventOn) {
			return;
		}

		//make screen pos 0.0f to 1.0f inorder program to detect where user's touching.
		Vector3 screenPos = Camera.main.ScreenToViewportPoint (new Vector3 (touchPos.x, touchPos.y, Camera.main.nearClipPlane));

		//get the screen pos 0.0f to 1.0f of the player.
		Vector3 playerPos = Camera.main.WorldToViewportPoint (GameManager.Instance.player.transform.position);
//		Debug.Log ("***********************playerPos: " + playerPos + ", screenPos: " + screenPos);

		//touching right of the screen (0.1f is the space)
		if (screenPos.x > playerPos.x + 0.1f && StageBlock.stageBlock != StageBlock.IS_STAGEBLOCK_RIGHT) {
//			Debug.Log ("right");
			girlAnimationObj.transform.localScale = new Vector3(-PLAYER_DEFAULT_SCALE, PLAYER_DEFAULT_SCALE, 1.0f);
			girlAnimationObj.transform.localPosition += new Vector3(7.0f, 0.0f, 0.0f);

		} 
		//touching left (0.1f is the space)
		else if (screenPos.x < playerPos.x - 0.1f && StageBlock.stageBlock != StageBlock.IS_STAGEBLOCK_LEFT) {
//			Debug.Log ("left");
			girlAnimationObj.transform.localScale = new Vector3(PLAYER_DEFAULT_SCALE, PLAYER_DEFAULT_SCALE, 1.0f);
			girlAnimationObj.transform.localPosition += new Vector3(-7.0f, 0.0f, 0.0f);
		}

		//throw motion if it hits enemy
		Ray ray = Camera.main.ScreenPointToRay(touchPos);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
//			Debug.Log ("OnTouchEnter: " + hit.collider.gameObject);
			EnemyAI enemyObj = hit.collider.gameObject.GetComponent<EnemyAI>();
			EnemyWeapon enemyWpnObj = hit.collider.gameObject.GetComponent<EnemyWeapon>();

			//attack on Enemy
			//right after instanticate, the charType has not been decided. detect by name.
			if(enemyObj != null && enemyObj.charType == EnemyAI.CharacterType.ENEMY) {
				float distance = Vector3.Distance(girlAnimationObj.transform.position, enemyObj.transform.position);
				if(distance < WEAPON_WIELD_DISTANCE) {
					girlAnimationObj.Play("throw");
					girlAnimationObj.PlayQueued("stand");
				}
				return ;
			}
			//attack on enemy weapon (only on bullets)
			else if(
				enemyWpnObj != null && 
				enemyWpnObj.owner != null && //just in case if the ower exists
				enemyWpnObj.owner.GetComponent<EnemyAI>().enemyMotion.GetComponent<EnemyMotionInterface>().closeWeaponPrefab == null) {
				float distance = Vector3.Distance(girlAnimationObj.transform.position, enemyWpnObj.transform.position);
				//enemy weapon on right
				if(girlAnimationObj.transform.position.x - enemyWpnObj.transform.position.x < 0) {
					girlAnimationObj.transform.localScale = new Vector3(-PLAYER_DEFAULT_SCALE, PLAYER_DEFAULT_SCALE, 1.0f);
				}
				//enemy weapon on left
				else {
					girlAnimationObj.transform.localScale = new Vector3(PLAYER_DEFAULT_SCALE, PLAYER_DEFAULT_SCALE, 1.0f);
				}

				if(distance < WEAPON_WIELD_DISTANCE) {
					girlAnimationObj.Play("throw");
					girlAnimationObj.PlayQueued("stand");
				}
				return ;
			}
		}

		girlAnimationObj.Play("walk");

		//expland the camera's field of view
		Camera.main.GetComponent<FollowScriptCamera>().ExpandFieldOfView();

		//change npc direction
		GameManager.Instance.DecideNpcPosition ();
	}

	void OnTouchUp(Vector2 touchPos) {
		//if menu is open
		if (MenuManager.Instance.IsMenuOpen ()) {
			return ;
		}

//		Debug.Log ("OnTouchUp");
		if (IsAbleToGetInput()) {
			girlAnimationObj.Play ("stand");
		}

		Camera.main.GetComponent<FollowScriptCamera> ().MinimizeFieldOfView ();


		isStartCharging = false;
	}

	void OnChargingStart(ChargedInfo cInfo) {
		//if menu is open
		if (MenuManager.Instance.IsMenuOpen ()) {
			return ;
		}

		if (GameManager.Instance.player == null) {
			return;
		}

		if (GameManager.Instance.crntNpcObj != null) {
			return;
		}

		if (girlAnimationObj == null || girlAnimationObj.IsPlaying ("pray")) {
			return;
		}

		Vector3 rayCastPos = cInfo.pos;
		rayCastPos.z = Camera.main.nearClipPlane; //奥行きを出す
			
		Vector3 screenPos = rayCastPos; //Camera.main.WorldToScreenPoint(rayCastPos);
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
//			Debug.DrawRay(ray.origin, ray.direction*2000, Color.red, 3.0f);
//			Debug.Log ("OnChargingStart hit Object: " + hit.collider.gameObject + ", cInfo.pos: " + cInfo.pos);
			if (hit.collider.gameObject.name.Contains ("Charlotte")) {
				isStartCharging = true;
				girlAnimationObj.Play ("pray");
			}
		}
	}

	//called when a charging event is detected
	void OnCharging(ChargedInfo cInfo){
		//if menu is open
		if (MenuManager.Instance.IsMenuOpen ()) {
			return ;
		}

		if (isStartCharging) {
//			Debug.Log ("OnCharging: " + cInfo.percent);
		}


	}
	
	//called when a charge event is ended
	void OnChargeEnd(ChargedInfo cInfo){
		//if menu is open
		if (MenuManager.Instance.IsMenuOpen ()) {
			return ;
		}

		//Debug.Log ("OnChargeEnd: " + cInfo.percent);
		if(cInfo.percent >= 1.0f && isStartCharging) {
			StartCoroutine(GameManager.Instance.SummonNpc(GameManager.Instance.playerParam.summonNpcIndex));
		}
		isStartCharging = false;
	}

	private bool IsAbleToGetInput() {
		if (girlAnimationObj == null) {
			return false;
		}

		if (EventListObject.isEventOn) {
			return false;
		}

		if (
			girlAnimationObj.IsPlaying ("throw") ||
			girlAnimationObj.IsPlaying ("pickup") ||
			girlAnimationObj.IsPlaying ("wakeup") ||
			girlAnimationObj.IsPlaying ("pickup_stand") || 
			girlAnimationObj.IsPlaying ("pray") ||
			girlAnimationObj.IsPlaying ("damage") ||
			girlAnimationObj.IsPlaying ("death")
		) {
			return false;
		}

		return true;
	}


}
