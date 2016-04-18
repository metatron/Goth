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
	public BoneAnimation boneAnimation;
	public float destroyTime = 2.0f;
	public float size = 1.0f;

	public TextureAtlas bloodAtlas;

	void Start () {
		transform.localScale = Vector3.one * size;

		int rnd = Random.Range (0, textureNameList.Count);
		string replaceId = bloodAtlas.GetTextureGUIDFromName ("blood1");
		string newTextureId = bloodAtlas.GetTextureGUIDFromName (textureNameList [rnd]);
		boneAnimation.ReplaceAnimationBoneTexture ("spill", "blood", bloodAtlas, replaceId, bloodAtlas, newTextureId);

		Destroy (gameObject, destroyTime);
	}
}
