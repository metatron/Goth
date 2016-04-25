using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : SingletonMonoBehaviourFast<MenuManager> {
	public GameObject currentActiveMenu;
	public GameObject ghostMenuPanel;
	public GameObject selectStageMenuPanel;
	public GameObject collectionMenuPanel;
	public GameObject collectionPopup;

	public GameObject okCancelPopup;
	public GameObject warningPopup;


	//store temporary collecking data.
	//after the pickup animation, window will popup.
	public static CollectionData pickupCollectionData;

	//for list collections
	private Dictionary<string, CollectionData> collectionPrefabDict = null;

	//Fading Object
	public FaderObject faderObj;

	void Start() {
		ghostMenuPanel.SetActive (false);
		selectStageMenuPanel.SetActive (false);
		collectionMenuPanel.SetActive (false);
		collectionPopup.SetActive (false);
		okCancelPopup.SetActive (false);
		warningPopup.SetActive (false);
	}


	//================= Ghost Menu ==============//

	public void OnGhostMenuButtonPressed() {
		ghostMenuPanel.SetActive (true);
		currentActiveMenu = ghostMenuPanel;
		int npcNum = GameManager.Instance.totalNpcList.Count;
		GameObject ghostPrefab = Resources.Load ("Prefabs/Menu/GhostNode") as GameObject;
		for (int i=0; i<npcNum; i++) {
			GameObject ghostNodeObj = (GameObject)Instantiate(ghostPrefab);
			ghostNodeObj.transform.SetParent(ghostMenuPanel.GetComponent<ScrollRect>().content.transform);
			ghostNodeObj.transform.localPosition = Vector3.zero;
			ghostNodeObj.transform.localScale = Vector3.one;
			CalculateCellSize (ghostNodeObj);
			ghostNodeObj.GetComponent<GhostNode>().SetGhostInfo(GameManager.Instance.totalNpcList[i], i);
		}
	}

	public void OnGhostMenuClose() {
		//destroy each charadter model first
		int npcNum = ghostMenuPanel.GetComponent<ScrollRect>().content.childCount;
		for (int i = 0; i < npcNum; i++) {
			ghostMenuPanel.GetComponent<ScrollRect> ().content.GetChild (i).GetComponent<GhostNode> ().DeleteFromMenu();
		}
		currentActiveMenu = null;
		ghostMenuPanel.SetActive (false);
	}

	public void resetSelectedButtonColor() {
		GhostNode[] ghostNodeList = ghostMenuPanel.GetComponent<ScrollRect> ().content.GetComponentsInChildren<GhostNode> ();
		int npcNum = ghostNodeList.Length;
		for (int i=0; i<npcNum; i++) {
			if (GameManager.Instance.playerParam.summonNpcIndex == i) {
				ghostNodeList[i].selectGhostButton.GetComponentInChildren<Text> ().color = Color.red;
			} else {
				ghostNodeList[i].selectGhostButton.GetComponentInChildren<Text> ().color = Color.black;
			}
		}
	}

	public bool IsMenuOpen() {
		if (currentActiveMenu != null) {
			return true;
		}
		return false;
	}

	//================= Select Stage Menu ==============//

	public void OnSelectStageMenuButtonOpened() {
		selectStageMenuPanel.SetActive (true);
		currentActiveMenu = selectStageMenuPanel;
		int stageNum = StageManager.Instance.stageList.Count;
		GameObject stagePrefab = Resources.Load ("Prefabs/Menu/StageNode") as GameObject;
		for (int i=0; i<stageNum; i++) {
			GameObject stageNodeObj = (GameObject)Instantiate(stagePrefab);
			stageNodeObj.transform.SetParent(selectStageMenuPanel.GetComponent<ScrollRect>().content.transform);
			stageNodeObj.transform.localPosition = Vector3.zero;
			stageNodeObj.transform.localScale = Vector3.one;
			CalculateCellSize (stageNodeObj);
			StageData stageData = StageManager.Instance.stageList [i];
			stageNodeObj.GetComponent<StageNode>().SetStageInfo(stageData);
		}
	}

	public void OnSelectStageMenuClose() {
		currentActiveMenu = null;
		selectStageMenuPanel.SetActive (false);
	}

	//================= Select Collection Menu ==============//

	public void OnCollectionMenuButtonOpened() {
		collectionMenuPanel.SetActive (true);
		currentActiveMenu = collectionMenuPanel;
		int collectionNum = GameManager.Instance.playerCollectedItemList.Count;
		GameObject collectionPrefab = Resources.Load ("Prefabs/Menu/CollectionNode") as GameObject;

		//init
		if (collectionPrefabDict == null) {
			collectionPrefabDict = new Dictionary<string, CollectionData> ();
		}

		for (int i=0; i<collectionNum; i++) {
			GameObject collectionNodeObj = (GameObject)Instantiate(collectionPrefab);
			collectionNodeObj.transform.SetParent(collectionMenuPanel.GetComponent<ScrollRect>().content.transform);
			collectionNodeObj.transform.localPosition = Vector3.zero;
			collectionNodeObj.transform.localScale = Vector3.one;
			CalculateCellSize (collectionNodeObj);

			//get collection prefab
			if (collectionPrefabDict.Count == 0) {
				string relativePath = "Prefabs/Collection/";
				CollectionData[] collectionPrefabArray = Resources.LoadAll<CollectionData> (relativePath);

				foreach (CollectionData prefab in collectionPrefabArray) {
					collectionPrefabDict.Add(prefab.id, prefab);
				}
			}
			CollectionData collectionData = collectionPrefabDict[GameManager.Instance.playerCollectedItemList [i]];

			collectionNodeObj.GetComponent<CollectionNode>().SetCollectionInfo(collectionData);
		}
	}

	public void OnCollectionMenuClose() {
		//destroy each charadter model first
		int npcNum = collectionMenuPanel.GetComponent<ScrollRect>().content.childCount;
		for (int i = 0; i < npcNum; i++) {
			collectionMenuPanel.GetComponent<ScrollRect> ().content.GetChild (i).GetComponent<CollectionNode> ().DeleteFromMenu();
		}

		currentActiveMenu = null;
		collectionMenuPanel.SetActive (false);
	}

	public void OpenCollectionPopup() {
		collectionPopup.SetActive (true);
		currentActiveMenu = collectionPopup;
		collectionPopup.GetComponent<CollectionPopup> ().InitializeCollectionPopup (pickupCollectionData);

	}

	public void OnCollectionPopupClose() {
		//play standup animation
		if (GameManager.Instance.player != null) {
			GameManager.instance.player.GetComponent<PlayerCharacter> ().playerAnimation.Play ("pickup_stand");
			GameManager.instance.player.GetComponent<PlayerCharacter> ().playerAnimation.PlayQueued ("stand");
		}
		Destroy (pickupCollectionData.gameObject);

		currentActiveMenu = null;
		pickupCollectionData = null; //remove currently picking up object
		collectionPopup.SetActive (false);
	}



	//================= FadOut ==============//
	public void ActivateAndStartFading(float sec, FaderObject.CompleteFadingDelegate OnCompDel) {
		faderObj.gameObject.SetActive (true);
		faderObj.StargFading (sec, OnCompDel);
	}



	//================= Cell size init ==============//
	/**
	 * 
	 * ディフォルトの画面サイズ、セルサイズを元に、現在の画面でのセルの縦横サイズを設定する。
	 * 
	 * 
	 */
	private void CalculateCellSize(GameObject cell) {
		//現画面サイズの比率を取得。
		Vector2 preSize = GetResizedSize ();

		cell.GetComponent<LayoutElement> ().minWidth = preSize.x;
		cell.GetComponent<LayoutElement> ().minHeight = preSize.y;
	}

	/**
	 * 
	 * リスト生成する際に使用。
	 * ノードが画面リサイズについてこれない。（Anchorが自由に設定できない）
	 * 
	 * ゴースト、コレクションのところではモノのリサイズにも使用。
	 * 
	 * 
	 */
	public static Vector2 GetResizedSize() {
		float expectedWidth = 480;
		float expectedHeight = 800;
		float defaultWidth = 300;
		float defaultHeight = 400;

		//画面に対する一つのセルの縦横比。
		float wRatio = defaultWidth / expectedWidth;
		float hRatio = defaultHeight / expectedHeight;

		Vector2 preSizeVec = Vector2.zero;
		//現画面でのセルの縦横
		preSizeVec.x = Screen.width * wRatio;
		preSizeVec.y = Screen.height * hRatio;

		return preSizeVec;
	}
}
