﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class BloodAnimation : MonoBehaviour {
	const int BLOOD_MAX_NUM				= 3;
	const int MAX_POSITION_DIS			= 80;
	const float INSTANTIATE_INTERVAL	= 0.2f;

	public List<string> textureNameList = new List<string> () {
		"blood1",
		"blood2",
		"blood3",
		"blood4",
	};
	public float destroyTime = 0.5f;
	public float size = 1.0f;

	public TextureAtlas bloodAtlas;
	public GameObject bloodPrefab;

	void Start () {
		StartCoroutine(InstantiateBloodParticle());
	}

	private IEnumerator InstantiateBloodParticle() {
		for (int i = 0; i < BLOOD_MAX_NUM; i++) {
			InitBloodParticle ();
			yield return new WaitForSeconds(INSTANTIATE_INTERVAL);
		}
	}


	private void  InitBloodParticle() {

		int rndX = Random.Range (-MAX_POSITION_DIS, MAX_POSITION_DIS);
		int rndY = Random.Range (-MAX_POSITION_DIS, MAX_POSITION_DIS);
		Vector3 pos = new Vector3 (transform.localPosition.x + rndX, transform.localPosition.y + rndY, transform.localPosition.z);

		GameObject bloodObj = (GameObject)Instantiate (bloodPrefab);
		bloodObj.transform.position = pos;
		bloodObj.transform.SetParent (transform);

		bloodObj.transform.localScale = Vector3.zero;
		SmoothMoves.Sprite sprite = bloodObj.GetComponent<SmoothMoves.Sprite> ();

		int rnd = Random.Range (0, textureNameList.Count);
		sprite.SetTextureName (textureNameList [rnd]);

		Destroy (bloodObj.gameObject, destroyTime);

		//サイズを
		iTween.ScaleTo(bloodObj.gameObject, iTween.Hash(
			"scale", Vector3.one * size,
			"time", destroyTime-0.1f //上記Destroyの前に破棄される可能性があるのでアニメーションの終了を若干早める。
		));


		//Start fading
//		iTween.ValueTo(gameObject, iTween.Hash(
//			"from", 1f,
//			"to", 0f,
//			"time", destroyTime-0.1f, //上記Destroyの前に破棄される可能性があるのでアニメーションの終了を若干早める。
//			"onupdatetarget", gameObject,
//			"onupdate", "OnUpdateFading",
//			"oncompletetarget", gameObject
//		));
	}

//	private void OnUpdateFading(float value) {
//		sprite.color = new Color (0, 0, 0, value);
//	}

}