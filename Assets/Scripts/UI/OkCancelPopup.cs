using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OkCancelPopup : MonoBehaviour {
	public Text titleText;
	public Text descText;

	public delegate void OnOkButtonPressed();
	public delegate void OnCancelButtonPressed();

	private OnOkButtonPressed okButtonPressedFunc;
	private OnCancelButtonPressed cancelButtonPressedFunc;

	public void InitializeCollectionPopup(string title, string desc, OnOkButtonPressed okButtonFunc, OnCancelButtonPressed cancelButtonFunc = null) {
		titleText.text = title;
		descText.text = desc;

		okButtonPressedFunc = okButtonFunc;
		cancelButtonPressedFunc = cancelButtonFunc;

		gameObject.SetActive (true);
	}

	public void OkButtonPressed() {
		okButtonPressedFunc ();

		gameObject.SetActive (false);
	}

	public void CancelButtonPressed() {
		if (cancelButtonPressedFunc != null) {
			cancelButtonPressedFunc ();
		}
		gameObject.SetActive (false);
	}

}
