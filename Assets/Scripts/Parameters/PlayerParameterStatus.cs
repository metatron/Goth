using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerParameterStatus : BaseParameterStatus {
	public PlayerParameterStatus() {
		type = GhostType.Player;

//		minVisibleDistance = 1000;
//		maxVisibleDistance = 2000;
//		crntVisibleDistance = 1000;
	}

//	[SerializeField]
//	public float maxVisibleDistance = 1000;

//	[SerializeField]
//	public int totalSpirit = 1000;

	[SerializeField]
	public int summonNpcIndex = -1;

	public static bool isHit = false;

}
