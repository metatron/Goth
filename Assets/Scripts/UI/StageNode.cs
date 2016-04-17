using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageNode : MonoBehaviour {
	public Text nameText;
	private StageData stageData;


	public void SetStageInfo (StageData stage) {
		stageData = stage;
		nameText.text = "Name: " + stageData.title;
	}

	/**
	 * 
	 * called from Stage Select Menu
	 * 
	 */
	public void InitStageWithFade() {
		MenuManager.Instance.ActivateAndStartFading(2.0f, () => { InitStage (); });
	}

	public void InitStage() {
		StageManager.Instance.InitStageReposPlayer (stageData.prefabPath);

		//close the window
		MenuManager.Instance.OnSelectStageMenuClose();
	}
}
