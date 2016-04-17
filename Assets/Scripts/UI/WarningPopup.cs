using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningPopup : MonoBehaviour {
	public Text titleText;
	public Text descText;

	public delegate void OnOkButtonPressed();

	private OnOkButtonPressed okButtonPressedFunc;

	public void InitializeWarningPopup(string title, string desc, OnOkButtonPressed okButtonFunc = null) {
		titleText.text = title;
		descText.text = desc;

		okButtonPressedFunc = okButtonFunc;

		gameObject.SetActive (true);
	}

	public void OkButtonPressed() {
		if (okButtonPressedFunc != null) {
			okButtonPressedFunc ();
		}

		gameObject.SetActive (false);
	}

}
