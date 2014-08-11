using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{	
	void Start(){
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("b")) {
			iTween.MoveTo(gameObject, iTween.Hash("y", -2));
		}
		
		
	}
}

