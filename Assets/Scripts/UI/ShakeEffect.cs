﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShakeEffect : MonoBehaviour {
	public RawImage blackObject;

	//1.0: totally black
	//0.0: totally transparent
	public const float ALPHA_MIN = 0.0f;//0.0f to 1.0f
	public const float ALPHA_MAX = 0.4f;//0.0f to 1.0f
	public const float CHANGE_TIME = 1.5f;

	private float startValue = ALPHA_MIN;
	private float endValue = ALPHA_MAX;

	// Use this for initialization
	void Start () {
//		iTween.ShakePosition (
//			gameObject,
//			iTween.Hash (
//				"amount", new Vector3(0.5f, 0.5f, 0.5f),
//				"time", 100.0f,
//				"islocal", true
//			)
//		);

		if (blackObject != null) {
			iTween.ValueTo(gameObject, 
				iTween.Hash(
					"from", startValue, 
					"to", endValue, 
					"time", CHANGE_TIME, 
					"onupdate", "UpdateBlackObjectAlpha",
					"oncomplete", "OnCompleteUpdate",
					"oncompletetarget", gameObject)
			);
		}
	}

	private void UpdateBlackObjectAlpha(float delta) {
		blackObject.color = new Color (0.0f, 0.0f, 0.0f, delta);
	}

	private void OnCompleteUpdate() {
		if (startValue == ALPHA_MIN) {
			startValue = ALPHA_MAX;
			endValue = ALPHA_MIN;
		} else {
			startValue = ALPHA_MIN;
			endValue = ALPHA_MAX;
		}

		iTween.ValueTo(gameObject, 
			iTween.Hash(
				"from", startValue, 
				"to", endValue, 
				"time", CHANGE_TIME, 
				"onupdate", "UpdateBlackObjectAlpha",
				"oncomplete", "OnCompleteUpdate",
				"oncompletetarget", gameObject)
		);

	}

	// Update is called once per frame
	void Update () {
	
	}
}