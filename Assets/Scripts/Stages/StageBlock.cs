using UnityEngine;
using System.Collections;

public class StageBlock : MonoBehaviour {
	public const int IS_STAGEBLOCK_NONE		= 0;
	public const int IS_STAGEBLOCK_LEFT		= 1;
	public const int IS_STAGEBLOCK_RIGHT	= 2;

	public static int stageBlock = IS_STAGEBLOCK_NONE;

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.GetComponent<PlayerCharacter> () == null) {
			return;
		}

		//if player hits this object it will set the flag according to the name of this object.
		if (gameObject.name.Contains ("right") || gameObject.name.Contains ("Right")) {
			stageBlock = IS_STAGEBLOCK_RIGHT;
		} else if (gameObject.name.Contains ("left") || gameObject.name.Contains ("Left")) {
			stageBlock = IS_STAGEBLOCK_LEFT;
		}
			
	}

	void OnTriggerExit(Collider other) {
		stageBlock = IS_STAGEBLOCK_NONE;
	}
}
