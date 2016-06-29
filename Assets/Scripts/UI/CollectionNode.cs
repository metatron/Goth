using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectionNode : MonoBehaviour {
	public Text nameText;
	public Text descText;
	private CollectionData collectionData;


	public void SetCollectionInfo (CollectionData collection) {
		collectionData = collection;
		nameText.text = collectionData.title;
		descText.text = collectionData.desc.Replace("\\n", "\n");

		GameObject collectionObj = Instantiate (collectionData.gameObject) as GameObject;
		collectionObj.transform.rotation = Quaternion.Euler(collection.collectionListSizeVec);
		collectionObj.transform.SetParent (transform);
		collectionObj.transform.localPosition = collection.collectionListPosVec; //new Vector3(0.0f, 0.0f, -10.0f);
		collectionObj.transform.localScale = collection.collectionListSizeVec;
		collectionObj.layer = 5; //UI layer.


		foreach(Transform child in collectionObj.GetComponentsInChildren<Transform>()) {
//			child.localScale = resizedScale;
			child.gameObject.layer = 5; //UI layer.
			Renderer renderer = child.GetComponent<Renderer>();
			if (renderer != null) {
				renderer.sortingOrder = 20;
			}
		}
	}

	public void DeleteFromMenu() {
		Destroy (gameObject);
	}
}
