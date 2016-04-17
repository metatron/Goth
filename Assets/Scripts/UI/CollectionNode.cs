using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectionNode : MonoBehaviour {
	public Text nameText;
	public Text descText;
	private CollectionData collectionData;


	public void SetCollectionInfo (CollectionData collection) {
		collectionData = collection;
		nameText.text = "Name: " + collectionData.title;
		descText.text = "Description: " + collectionData.desc;

		GameObject collectionObj = Instantiate (collectionData.gameObject) as GameObject;
		collectionObj.transform.SetParent (transform);
		collectionObj.transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);
		collectionObj.transform.localScale = collectionData.resizeVec;
		collectionObj.layer = 5; //UI layer.
		foreach(Transform child in collectionObj.GetComponentsInChildren<Transform>()) {
			child.localScale = collectionData.resizeVec;
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
