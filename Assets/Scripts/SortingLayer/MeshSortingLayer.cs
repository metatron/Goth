using UnityEngine;
using System.Collections;

/**
 * 
 * Use this on BoneAnimation Object.
 * the SortingLayer script will gives the error if the object has SmoothMoves.BoneAnimation.
 * 
 * 
 */
public class MeshSortingLayer : MonoBehaviour {
	public string sortingLayerName="Default";
	public int sortingOrder=0;

	void Awake() {
		Renderer renderer = GetComponent<Renderer> ();
		if (renderer != null) {
			renderer.sortingLayerName = sortingLayerName;
			renderer.sortingOrder = sortingOrder;
		}
	}

	public void SetSortingLayerNameAndOrder(string name, int order) {
		Renderer renderer = GetComponent<Renderer> ();
		if (renderer != null) {
			sortingLayerName = name;
			sortingOrder = order;
			renderer.sortingLayerName = sortingLayerName;
			renderer.sortingOrder = sortingOrder;
		}
	}
}
