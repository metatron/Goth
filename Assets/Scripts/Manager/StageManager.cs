﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class StageManager : SingletonMonoBehaviourFast<StageManager> {
	public List<StageData> stageList = new List<StageData> ();

	public static EventListObject crntEvent;

	void Start() {
	} 

	public StageData InitStage(int level) {
		if (stageList.Count <= 0) {
			Debug.LogError("Stage List is 0.");
			return null;
		}

		PlayerParameterStatus player = GameManager.Instance.player.GetComponent<PlayerCharacter> ().status;
		if (player == null) {
			Debug.LogError("Player not yet initialized during the stage load.");
			return null;
		}

		//collect stages that can be played.
		List<StageData> playableStageList = new List<StageData> ();
		foreach (StageData stage in stageList) {
			if(stage.spawnerList.Count <= 0) {
				Debug.LogError("stage: " + stage.name + " should be set at least one spawner.");
				return null;
			}
			if(stage.enemyStatusList.Count <= 0) {
				Debug.LogError("stage: " + stage.name + " should be set at least one enemy.");
				return null;
			}

			if(stage.level <= player.level) {
				playableStageList.Add(stage);
			}
		}

		//getting path of the stage prefab
		int rnd = Random.Range (0, playableStageList.Count);
		StageData loadingStage = playableStageList [rnd];

		//init enemy on spawner
		//InitStageEnemies (loadingStage);


		return InstantiateStage (loadingStage.prefabPath);
	}

	public StageData InstantiateStage(string path) {
		GameObject stageObject = (GameObject)Instantiate (Resources.Load(path));
		stageObject.transform.position = Vector3.zero;
		GameManager.Instance.crntStageData = stageObject.GetComponent<StageData> ();
		return stageObject.GetComponent<StageData>();
	}

	public void InitStageReposPlayer(string path, string animAftrStgLoad = null) {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage (path);

		//move character to the original position
		GameManager.Instance.player.transform.position = Vector3.zero;

		//move camera
		Camera.main.transform.position = new Vector3(0.0f, 550.0f, -1200.0f);

		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();

		if (!string.IsNullOrEmpty (animAftrStgLoad) && GameManager.Instance.player != null) {
			GameManager.Instance.player.GetComponent<BoneAnimation> ().Play (animAftrStgLoad);
		}
	}

	public void DeleteCurrentStage() {
		//delete the stage data
		Destroy (GameManager.Instance.crntStageData.gameObject);
		GameManager.Instance.crntStageData = null;

		//delete the enemy data
		GameManager.Instance.DeleteAllEnemies();
	}
}