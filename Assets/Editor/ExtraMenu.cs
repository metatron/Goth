using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ExtraMenu : MonoBehaviour {

	// Add a menu item called "Double Mass" to a Rigidbody's context menu.
	[MenuItem ("Tools/Goth/AddNpcData/Queen")]
	static void AddQueenChar() {
		AddNpcData (BaseParameterStatus.GhostType.Queen);
	}
	[MenuItem ("Tools/Goth/AddNpcData/TiedGhost")]
	static void AddTiedGhostChar() {
		AddNpcData (BaseParameterStatus.GhostType.TiedGhost);
	}
	[MenuItem ("Tools/Goth/AddNpcData/Nurse")]
	static void AddNurseChar() {
		AddNpcData (BaseParameterStatus.GhostType.Nurse);
	}
	[MenuItem ("Tools/Goth/AddNpcData/WillOWisp")]
	static void AddWillOWispChar() {
		AddNpcData (BaseParameterStatus.GhostType.WillOWisp);
	}

	private static void AddNpcData (BaseParameterStatus.GhostType charaType) {
		NpcParameterStatus npcStatus = new NpcParameterStatus ();
		npcStatus.type = charaType;
		GameManager.Instance.totalNpcList.Add (npcStatus);
	}

	// Delete all Player status
	[MenuItem ("Tools/Goth/DeleteAllData")]
	static void DeleteAllData () {
		SaveLoadStatus.DeleteAllData ();
	}

	// Increase Attack
	[MenuItem ("Tools/Goth/IncreaseAttack")]
	static void IncreaseAttackData () {
		GameManager.Instance.player.GetComponent<PlayerCharacter> ().status.crntAtk += 100;
		GameManager.Instance.player.GetComponent<PlayerCharacter> ().currentWeapon.GetComponentInChildren<Weapon> ().attack += 100;
	}

	//**************** Load Stages ****************//

	[MenuItem ("Tools/Goth/LoadStage/Castle")]
	static void LoadStage_Castle () {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage ("Prefabs/Stage/Stage_Castle");
		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();
	}

	[MenuItem ("Tools/Goth/LoadStage/HangTree")]
	static void LoadStage_Hang () {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage ("Prefabs/Stage/Stage_Hang");
		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();
	}

	[MenuItem ("Tools/Goth/LoadStage/Desert")]
	static void LoadStage_Desert () {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage ("Prefabs/Stage/Stage_Desert");
		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();
	}

	[MenuItem ("Tools/Goth/LoadStage/Dungeon")]
	static void LoadStage_Dungeon () {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage ("Prefabs/Stage/Stage_Dungeon");
		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();
	}
}
