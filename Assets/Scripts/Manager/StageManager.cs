using UnityEngine;
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

		//init skybox
		GameObject skyBoxCameraObj = GameObject.FindWithTag("SkyBoxCamera");
		skyBoxCameraObj.GetComponent<Skybox> ().material = GameManager.Instance.crntStageData.skyBoxMat;

		return stageObject.GetComponent<StageData>();
	}

	public void InitStageReposPlayer(string path, string animAftrStgLoad = null) {
		//delete enemy related objests
		StageManager.Instance.DeleteCurrentStage ();
		//instantilate stage data
		StageManager.Instance.InstantiateStage (path);

		//move character to the original position
		GameManager.Instance.player.transform.position = Vector3.zero;

		//reset HP if and only if the palyer is going back to HOME stage
		if (path.Contains ("Stage_Home")) {
			GameManager.Instance.playerParam.crntHp = GameManager.Instance.playerParam.GetBaseHp ();
		}

		//move camera
		Camera.main.transform.position = new Vector3(0.0f, 550.0f, -1200.0f);

		//move npc if exists
		if (GameManager.Instance.crntNpcObj != null) {
			float y = GameManager.Instance.crntNpcObj.GetComponent<FollowScript> ().fixedPos.y;
			GameManager.Instance.crntNpcObj.transform.position = new Vector3 (0.0f, y, 0.0f);//GameManager.Instance.player.transform.position;
		}

		//after load status, remove collected items
		GameManager.Instance.crntStageData.InitStageCollectionItems();

		if (!string.IsNullOrEmpty (animAftrStgLoad) && GameManager.Instance.player != null) {
			GameManager.Instance.player.GetComponent<BoneAnimation> ().Play (animAftrStgLoad);
			GameManager.Instance.player.GetComponent<BoneAnimation> ().PlayQueued ("stand");
		}
	}

	public void DeleteCurrentStage() {
		//delete the stage data
		Destroy (GameManager.Instance.crntStageData.gameObject);
		GameManager.Instance.crntStageData = null;

		//delete the enemy data
		GameManager.Instance.DeleteAllEnemies();
	}


	/**
	 * 
	 * this function will increase the brightness of the stage.
	 * the brightness can be increased only from NPC skill.
	 * 
	 * will be called during the Summoning, UnSommon, and Stage Init.
	 * 
	 * if the brightnessInc == 0, the default value will be set
	 * from the prefab
	 * 
	 */
	public void UpdateStageBrightnessBySkill(float brightnessInc) {
		if (GameManager.Instance.crntStageData == null) {
			return ;
		}

		//if brightness == 0, set default
		GameObject stageDataPrefab = (GameObject)Resources.Load (GameManager.Instance.crntStageData.prefabPath) as GameObject;
		GameManager.Instance.crntStageData.minStageLight = stageDataPrefab.GetComponent<StageData>().minStageLight;
		GameManager.Instance.crntStageData.maxStageLight = stageDataPrefab.GetComponent<StageData>().maxStageLight;

		//if theres any skills that affects brightnessInc, calculate it
		if (brightnessInc > 0) {
			GameManager.Instance.crntStageData.minStageLight -= brightnessInc;
			GameManager.Instance.crntStageData.maxStageLight -= brightnessInc;

		}
		//check and decide the min and max val
		GameManager.Instance.crntStageData.minStageLight = Mathf.Max(0.0f, GameManager.Instance.crntStageData.minStageLight);
		GameManager.Instance.crntStageData.maxStageLight = Mathf.Min (1.0f, GameManager.Instance.crntStageData.maxStageLight);

		//update the  alpha value
		Debug.LogError("be4: " + ShakeEffect.ALPHA_MIN + ", " + ShakeEffect.ALPHA_MAX);
		ShakeEffect.ALPHA_MIN = GameManager.Instance.crntStageData.minStageLight;
		ShakeEffect.ALPHA_MAX = GameManager.Instance.crntStageData.maxStageLight;
		Debug.LogError("aft: " + ShakeEffect.ALPHA_MIN + ", " + ShakeEffect.ALPHA_MAX);
	}
}
