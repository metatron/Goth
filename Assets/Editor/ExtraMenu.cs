using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using SmoothMoves;
using Soomla;
using Soomla.Store;

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
	[MenuItem ("Tools/Goth/AddNpcData/TeddyBear")]
	static void AddTeddyBearChar() {
		AddNpcData (BaseParameterStatus.GhostType.TeddyBear);
	}

	private static void AddNpcData (BaseParameterStatus.GhostType charaType) {
		GameObject ghostPrefab = BaseParameterStatus.GetPrefabByType (charaType);
		NpcParameterStatus npcStatus = CopyBaseStatus2Npc(ghostPrefab.GetComponent<EnemyAI> ().status);
		GameManager.Instance.totalNpcList.Add (npcStatus);
	}

	private static NpcParameterStatus CopyBaseStatus2Npc(BaseParameterStatus baseStatus) {
		NpcParameterStatus status = new NpcParameterStatus ();

		status.minHp = baseStatus.minHp;
		status.maxHp = baseStatus.maxHp;

		status.minAtk = baseStatus.minAtk;
		status.maxAtk = baseStatus.maxAtk;

		status.minMoveSpeed = baseStatus.minMoveSpeed;
		status.maxMoveSpeed = baseStatus.maxMoveSpeed;

		status.minAtkSpeed = baseStatus.minAtkSpeed;
		status.maxAtkSpeed = baseStatus.maxAtkSpeed;

		status.minVisibleInc = baseStatus.minVisibleInc;
		status.maxVisibleInc = baseStatus.maxVisibleInc;

		status.level = baseStatus.level;

		status.type = baseStatus.type;
		status.pattern = baseStatus.pattern;

		status.skillPathList = baseStatus.skillPathList;

		return status;
	}

	// Delete all Player status
	[MenuItem ("Tools/Goth/DeleteAllData")]
	static void DeleteAllData () {
		SaveLoadStatus.DeleteAllData ();
		//reset soomla
		Soomla.KeyValueStorage.Purge ();
	}

	// Delete all Player status except soomla
	[MenuItem ("Tools/Goth/DeletePlayerData (w/out Soomla)")]
	static void DeletePlayerData () {
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

	[MenuItem ("Tools/Goth/LoadStage/Torture")]
	static void LoadStage_Torture () {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage ("Prefabs/Stage/Stage_Dungeon_Torture");
		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();
	}

	[MenuItem ("Tools/Goth/LoadStage/Ruin")]
	static void LoadStage_Ruin () {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage ("Prefabs/Stage/Stage_Ruin");
		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();
	}


	[MenuItem ("Tools/Goth/RefreshAtlas")]
	static void RefreshAtlas () {
		StageData stage;
	}

}
