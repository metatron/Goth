using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class BloodAnimation : MonoBehaviour {
	public List<string> textureNameList = new List<string> () {
		"blood1",
		"blood2",
		"blood3",
		"blood4",
	};
	private SmoothMoves.Sprite sprite;
	public float destroyTime = 2.0f;
	public float size = 1.0f;

	public TextureAtlas bloodAtlas;

	void Start () {
		transform.localScale = Vector3.zero;
		sprite = GetComponent<SmoothMoves.Sprite> ();

		int rnd = Random.Range (0, textureNameList.Count);
		sprite.SetTextureName (textureNameList [rnd]);

		Destroy (gameObject, destroyTime);

		//サイズを
		iTween.ScaleTo(gameObject, iTween.Hash(
			"scale", Vector3.one * size,
			"time", destroyTime-0.1f //上記Destroyの前に破棄される可能性があるのでアニメーションの終了を若干早める。
		));


		//Start fading
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", 255f,
			"to", 0f,
			"time", destroyTime-0.1f, //上記Destroyの前に破棄される可能性があるのでアニメーションの終了を若干早める。
			"onupdatetarget", gameObject,
			"onupdate", "OnUpdateFading",
			"oncompletetarget", gameObject
		));
	}

	private void OnUpdateFading(float value) {
		sprite.color = new Color (0, 0, 0, value);
	}
}
