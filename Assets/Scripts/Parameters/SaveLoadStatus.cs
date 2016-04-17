using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class SaveLoadStatus {
	public const string SAVEPATH_PLAYER = "/PlayerStatus";
	public const string SAVEPATH_NPCLIST = "/NpcStatusList";


	[Serializable]
	public class PlayerParameters {
		[SerializeField]
		public PlayerParameterStatus playerParams;
		[SerializeField]
		public List<NpcParameterStatus> npcParamList;
		[SerializeField]
		public List<string> playerCollectedItemIDList;
		[SerializeField]
		public List<string> playerDoneEventIDList;


		public PlayerParameters() {
			playerParams = GameManager.Instance.player.GetComponent<PlayerCharacter> ().status;
			npcParamList = GameManager.Instance.totalNpcList;

			playerCollectedItemIDList = GameManager.Instance.playerCollectedItemList;
			playerDoneEventIDList = GameManager.Instance.eventDoneList;
		}

		public void SetPlayerParameters() {
			GameManager.Instance.player.GetComponent<PlayerCharacter> ().status = playerParams;
			GameManager.Instance.totalNpcList = npcParamList;

			GameManager.Instance.playerCollectedItemList = playerCollectedItemIDList;
			GameManager.Instance.eventDoneList = playerDoneEventIDList;

			//init weapon atk
			GameManager.Instance.player.GetComponent<PlayerCharacter> ().InitWeaponStatus();
		}
	}


	public static void SaveUserParameters() {
		if (GameManager.Instance.player != null) {
			PlayerParameters parameters = new PlayerParameters ();
			string statusStr = JsonUtility.ToJson (parameters);

			BinaryFormatter bf = new BinaryFormatter ();
			Debug.Log ("save: " + Application.persistentDataPath + SAVEPATH_PLAYER);
			FileStream file = File.Create (Application.persistentDataPath + SAVEPATH_PLAYER);
			bf.Serialize (file, statusStr);
			file.Close ();
		}
	}

	public static void LoadUserParameters() {
		BinaryFormatter bf = new BinaryFormatter ();

		if(!File.Exists(Application.persistentDataPath + SAVEPATH_PLAYER)) {
			return ;
		}

		FileStream file = File.Open (Application.persistentDataPath + SAVEPATH_PLAYER, FileMode.Open);

		if(file.Length == 0) {
			return ;
		}

		string statusStr = (string)bf.Deserialize (file);
		file.Close ();

		if (statusStr.Length > 0) {
			PlayerParameters playerParams = JsonUtility.FromJson<PlayerParameters> (statusStr);
			playerParams.SetPlayerParameters ();
		}
	}

	public static void DeleteAllData() {
		if(!File.Exists(Application.persistentDataPath + SAVEPATH_PLAYER)) {
			return ;
		}

		File.Delete (Application.persistentDataPath + SAVEPATH_PLAYER);
	}
}
