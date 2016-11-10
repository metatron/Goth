using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {
	public GameObject headObj;
	public GameObject bodyObj;
	public GameObject leftArmObj;
	public GameObject rightArmObj;
	public GameObject leftLegObj;
	public GameObject rightLegObj;

	public GameObject headParticle;
	public GameObject armParticle;
	public GameObject bodyParticle;
	public GameObject legParticle;

	// Use this for initialization
	void Start () {
		//head
		GameObject tmpHeadParticle = (GameObject)Instantiate (headParticle);
		tmpHeadParticle.transform.parent = headObj.transform;
		tmpHeadParticle.transform.localPosition = Vector3.zero;

		//body
		GameObject tmpBodyParticle = (GameObject)Instantiate (armParticle);
		tmpBodyParticle.transform.parent = bodyObj.transform;
		tmpBodyParticle.transform.localPosition = Vector3.zero;

		//rightarm
		GameObject tmpRightArmParticle = (GameObject)Instantiate (armParticle);
		tmpRightArmParticle.transform.parent = rightArmObj.transform;
		tmpRightArmParticle.transform.localPosition = new Vector3(-25.0f, -53.0f, 0.0f);
		//leftarm
		GameObject tmpLeftArmParticle = (GameObject)Instantiate (armParticle);
		tmpLeftArmParticle.transform.parent = leftArmObj.transform;
		tmpLeftArmParticle.transform.localPosition = new Vector3(10.0f, -53.0f, 0.0f);

		//rightleg
		GameObject tmpRightLegParticle = (GameObject)Instantiate (armParticle);
		tmpRightLegParticle.transform.parent = rightLegObj.transform;
		tmpRightLegParticle.transform.localPosition = new Vector3(-22.0f, -86.0f, 0.0f);
		//leftleg
		GameObject tmpLeftLegParticle = (GameObject)Instantiate (armParticle);
		tmpLeftLegParticle.transform.parent = leftLegObj.transform;
		tmpLeftLegParticle.transform.localPosition = new Vector3(20.0f, -86.0f, 0.0f);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
