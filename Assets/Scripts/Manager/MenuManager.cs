using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;
using Soomla.Store;

public class MenuManager : SingletonMonoBehaviourFast<MenuManager> {
	public GameObject currentActiveMenu;
	public GameObject ghostMenuPanel;
	public GameObject selectStageMenuPanel;
	public GameObject collectionMenuPanel;
	public GameObject collectionPopup;
	public GameObject purchaseMenuPanel;

	public GameObject okCancelPopup;
	public GameObject warningPopup;

	//Blocks the Touch on Spirit Purchase.
	public GameObject uiTouchBlocker;


	//store temporary collecking data.
	//after the pickup animation, window will popup.
	public static CollectionData pickupCollectionData;

	//for list collections
	private Dictionary<string, CollectionData> collectionPrefabDict = null;

	//Fading Object
	public FaderObject faderObj;

	//GrundgeEffect
	public ShakeEffect grundgeObj;

	void Start() {
		ghostMenuPanel.SetActive (false);
		selectStageMenuPanel.SetActive (false);
		collectionMenuPanel.SetActive (false);
		collectionPopup.SetActive (false);
		okCancelPopup.SetActive (false);
		warningPopup.SetActive (false);
		uiTouchBlocker.SetActive (false);

		//initialize soomla
		SoomlaStore.Initialize (new ShopItemAssets ());

		//init the store event handler
		StoreEvents.OnMarketPurchase += OnMarketPurchase;
		StoreEvents.OnCurrencyBalanceChanged += OnCurrencyBalanceChanged;
		StoreEvents.OnMarketPurchaseCancelled += OnMarketPurchaseCancelled;
		StoreEvents.OnUnexpectedStoreError += OnUnexpectedStoreError;
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
//			CalculateCellSize (ghostNodeObj);
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
//			CalculateCellSize (stageNodeObj);
			StageData stageData = StageManager.Instance.stageList [i];
			stageNodeObj.GetComponent<StageNode>().SetStageInfo(stageData);
		}
	}

	public void OnSelectStageMenuClose() {
		currentActiveMenu = null;
		selectStageMenuPanel.SetActive (false);

		//stage close logic
		StageMenuClose ();
	}

	private void StageMenuClose() {
		//move player
		Vector3 playerPos = GameManager.Instance.player.transform.position;
		iTween.MoveTo (GameManager.Instance.player,
			iTween.Hash (
				"position", new Vector3(playerPos.x-250.0f, playerPos.y, playerPos.z),
				"time", 1.0f
			));
		//shut the door
		if (GameManager.Instance.crntStageData.homeDoorObj != null) {
			GameManager.Instance.crntStageData.homeDoorObj.GetComponentInChildren<HomeDoor> ().PlayReversedAnimation ();
		}
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
//			CalculateCellSize (collectionNodeObj);

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


	//================= Select Purchase Spirit Menu ==============//

	public void OnPurchaseMenuButtonOpened() {
		purchaseMenuPanel.SetActive (true);
		currentActiveMenu = purchaseMenuPanel;
	}

	public void OnPurchaseSpirit(GameObject button) {
		//detect which button is pressed.
		string[] splittedName = button.name.Split ('_');
		if (splittedName.Length != 2) {
			Debug.LogError ("Error on button Name! " + button);
		}

		//turn of touch blocker before purchase for avoid double click
		uiTouchBlocker.SetActive (true);

		string itemMod = splittedName[1];
		switch (itemMod) {
		case "100":
			StoreInventory.BuyItem (ShopItemAssets.SPIRIT100PACK_ITEMID);
			break;
		default:
			Debug.LogError ("Error Cannot find itemMod: " + itemMod);
			break;
		}
	}


	public void OnPurchaseMenuClose() {
		currentActiveMenu = null;
		purchaseMenuPanel.SetActive (false);
	}


	//================= Purchase Event Handling Processes ==============//

	private void OnMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra) {
		warningPopup.GetComponent<WarningPopup> ().InitializeWarningPopup ("Purchase Conpleted", "Purchasing " + pvi.Name + " Completed.");
		uiTouchBlocker.SetActive (false);
	}

	private void OnCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded) {
	}

	private void OnMarketPurchaseCancelled(PurchasableVirtualItem pvi) {
		warningPopup.GetComponent<WarningPopup> ().InitializeWarningPopup ("Purchase Canceled", "Purchase Canceled.");
		uiTouchBlocker.SetActive (false);
	}

	private void OnUnexpectedStoreError(int errorCode) {
		warningPopup.GetComponent<WarningPopup> ().InitializeWarningPopup ("Purchase Error", "Something went wrong during the purchase.\nError Code: " + errorCode);
		uiTouchBlocker.SetActive (false);
	}
		


	//================= FadOut ==============//
	public void ActivateAndStartFading(float sec, FaderObject.CompleteFadingDelegate OnCompDel) {
		faderObj.gameObject.SetActive (true);
		faderObj.StargFading (sec, OnCompDel);
	}



}
