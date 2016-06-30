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
		collectionObj.transform.SetParent (transform);
		//resize and relocate the collection prefab.
		collectionObj.transform.localPosition = collection.collectionListPosVec; //new Vector3(0.0f, 0.0f, -10.0f);
		collectionObj.transform.localRotation = Quaternion.Euler(collection.collectionListRotVec);
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
