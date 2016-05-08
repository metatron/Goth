using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectionPopup : MonoBehaviour {
	public Text nameText;
	public Text descText;
	public CollectionData collectionData;

	public void InitializeCollectionPopup(CollectionData data) {
		collectionData = data;
		nameText.text = collectionData.title;
		descText.text = collectionData.desc.Replace("\\n", "\n");
	}
}
