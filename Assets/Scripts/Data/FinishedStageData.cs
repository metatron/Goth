using UnityEngine;
using System;

/**
 * 
 * used to store how many times the user has finished the stage.
 * 
 * 
 */
[Serializable]
public class FinishedStageData {
	[SerializeField]
	public string stgId;

	[SerializeField]
	public int clearedNum;

	public FinishedStageData(string stageId, int clearedNum) {
		this.stgId = stageId;
		this.clearedNum = clearedNum;
	}

	public FinishedStageData(string stageId) {
		this.stgId = stageId;
		this.clearedNum++;
	}
}
