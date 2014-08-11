using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("b")) {
			if (gameObject.name == "MetalDoorBottom") { 
				iTween.MoveTo(gameObject, iTween.Hash("y", -4));
			}
		}
		
		
	}
}
