using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class GhostNode : MonoBehaviour {
	public int index;
	public Text nameText;
	public Text attackText;
	public Text hpText;
	private EnemyParameterStatus.GhostType type = EnemyParameterStatus.GhostType.TiedGhost;
	private GameObject ghostObj;
	private NpcParameterStatus npcStatus;

	public GameObject selectGhostButton;

	private static List<float> ghostYPosList = new List<float>() {
		0.0f,		//TiedGhost
		-140.0f,	//Nurse
		-140.0f,	//WillOWisp
		-140.0f		//Queen
	};


	public void SetGhostInfo (NpcParameterStatus npcStatus, int index) {
		this.index = index;
		GameObject ghostPrefab = npcStatus.GetPrefab ();
		ghostObj = (GameObject)Instantiate(ghostPrefab);
		ghostObj.GetComponent<EnemyAI>().transform.parent = transform;

		if (GhostNode.ghostYPosList.Count != System.Enum.GetNames (typeof(BaseParameterStatus.GhostType)).Length-1) {
			Debug.LogError ("ghostYPosList count and GhostType count is Wrong! check the definition! " + (GhostNode.ghostYPosList.Count) + "," + (System.Enum.GetNames (typeof(BaseParameterStatus.GhostType)).Length-1));
		}
		ghostObj.transform.localPosition = new Vector3(0.0f, GhostNode.ghostYPosList[(int)npcStatus.type], -10.0f);
		ghostObj.GetComponent<EnemyAI>().defaultSizeVec = ghostObj.GetComponent<EnemyAI> ().ghostListSizeVec;
//		ghostObj.GetComponent<EnemyAI>().defaultSizeVec = new Vector3 (0.3f, 0.3f, 1.0f);
//		ghostObj.layer = 5;
		ghostObj.GetComponent<MeshSortingLayer> ().SetSortingLayerNameAndOrder("UI", 5);
//		ghostObj.GetComponent<MeshSortingLayer> ().sortingLayerName = "UI";
		ghostObj.GetComponent<EnemyAI> ().SetAsMenuUI ();

		this.npcStatus = npcStatus;

		nameText.text = "Name: " + npcStatus.type;
		attackText.text = "Attack: " + npcStatus.GetBaseAtk();
		hpText.text = "HP: " + npcStatus.GetBaseHp();

		//change color of the select button if it is selected already
		MenuManager.Instance.resetSelectedButtonColor();
	}

	public void DeleteFromMenu() {
		Destroy(ghostObj);
		Destroy (gameObject);
	}

	public void OnSelectGhostPressed() {
		MenuManager.Instance.okCancelPopup.GetComponent<OkCancelPopup> ().InitializeCollectionPopup ("Select Summoning Ghost", "Set as Summoning Ghost?", SetMainGhost);
	}

	private void SetMainGhost() {
		Debug.Log ("Setting MainGhost to: " + index);
		GameManager.Instance.playerParam.summonNpcIndex = index;
		SaveLoadStatus.SaveUserParameters ();

		//reset text color
		MenuManager.Instance.resetSelectedButtonColor();

		//destroy if theres any npc exists.
		if (GameManager.Instance.crntNpcObj != null) {
			Destroy (GameManager.Instance.crntNpcObj);
			GameManager.Instance.crntNpcObj = null;
		}
	}

	public void OnLevelUpButtonPressed() {
		MenuManager.Instance.okCancelPopup.GetComponent<OkCancelPopup> ().InitializeCollectionPopup ("Level Up", "Do you want to Levelup?", LevelUpCharacter);
	}

	private void LevelUpCharacter() {
		int maxLevel = GhostLevelMaster.GetMaxLevel (npcStatus);
		if (npcStatus.level + 1 < maxLevel) {
			int cost2LevelUp =	GhostLevelMaster.CalculateCost (npcStatus.cost, npcStatus.level + 1, npcStatus.rarity);
			int nextHp = 		GhostLevelMaster.CalculateLevelParams (npcStatus.minHp, npcStatus.maxHp, npcStatus.level + 1, maxLevel, npcStatus.pattern);
			int nextAtk =		GhostLevelMaster.CalculateLevelParams (npcStatus.minAtk, npcStatus.maxAtk, npcStatus.level + 1, maxLevel, npcStatus.pattern);
			int nextMoveSpeed =	GhostLevelMaster.CalculateLevelParams (npcStatus.minMoveSpeed, npcStatus.maxMoveSpeed, npcStatus.level + 1, maxLevel, npcStatus.pattern);
			int nextAtkSpeed =	GhostLevelMaster.CalculateLevelParams (npcStatus.minAtkSpeed, npcStatus.maxAtkSpeed, npcStatus.level + 1, maxLevel, npcStatus.pattern);
			Debug.Log (
				"cost2LevelUp: " + cost2LevelUp + 
				", nextHp: " + nextHp + 
				", nextAtk: " + nextAtk + 
				", nextMoveSpeed: " + nextMoveSpeed + 
				", nextAtkSpeed: " + nextAtkSpeed
			);

			//check if the user has money
			if (GameManager.Instance.playerParam.totalSpirit >= cost2LevelUp) {
				//update
				npcStatus.UpdateStatus (nextAtk, nextHp, nextMoveSpeed, nextAtkSpeed, npcStatus.level + 1);
				//decrement money
				GameManager.Instance.playerParam.totalSpirit -= cost2LevelUp;
				//save to the disk
				SaveLoadStatus.SaveUserParameters ();
			} else {
				//TODO: Popup window
				MenuManager.Instance.warningPopup.GetComponent<WarningPopup>().InitializeWarningPopup("Not Enought Spirits", "");
			}
		}
	}


}
