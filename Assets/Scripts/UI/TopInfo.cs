using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopInfo : MonoBehaviour {
	public enum TOPINFO_TYPE: int {
		HP = 0,
		ATK = 1,
		BRIGHT = 2,
		SPIRIT = 3
	}

	public enum TOPINFO_TURNONOFF: int {
		OFF = 0,
		ON = 1
	}

	public GameObject hpUpGameObj;
	private Text hpUpGameTxt;

	public GameObject atkUpGameObj;
	private Text atkUpGameTxt;

	public GameObject brightUpGameObj;
	private Text brightUpGameTxt;

	public GameObject spiritGameObj;
	private Text spiritGameTxt;

	public void TurnOnOff(TOPINFO_TURNONOFF onOff, TOPINFO_TYPE type) {
		switch (type) {
		case TOPINFO_TYPE.HP:
			if (onOff == TOPINFO_TURNONOFF.OFF) {
				hpUpGameObj.SetActive (false);
			} else {
				hpUpGameObj.SetActive (true);
			}
			break;

		case TOPINFO_TYPE.ATK:
			if (onOff == TOPINFO_TURNONOFF.OFF) {
				atkUpGameObj.SetActive (false);
			} else {
				atkUpGameObj.SetActive (true);
			}
			break;

		case TOPINFO_TYPE.BRIGHT:
			if (onOff == TOPINFO_TURNONOFF.OFF) {
				brightUpGameObj.SetActive (false);
			} else {
				brightUpGameObj.SetActive (true);
			}
			break;

		case TOPINFO_TYPE.SPIRIT:
			if (onOff == TOPINFO_TURNONOFF.OFF) {
				spiritGameObj.SetActive (false);
			} else {
				spiritGameObj.SetActive (true);
			}
			break;

		}
	}

	public void UpdateHpUp(float value, TOPINFO_TYPE type) {
		double val = System.Math.Round ((double)value, 2, System.MidpointRounding.AwayFromZero);
		string sign = "-";
		if (val >= 0) {
			sign = "+";
		}

		switch (type) {
		case TOPINFO_TYPE.HP:
			hpUpGameTxt.text = sign + val;
			break;

		case TOPINFO_TYPE.ATK:
			atkUpGameTxt.text = sign + val;
			break;

		case TOPINFO_TYPE.BRIGHT:
			brightUpGameTxt.text = sign + val;
			break;

		case TOPINFO_TYPE.SPIRIT:
			spiritGameTxt.text = sign + val;
			break;

		}
	}
}
