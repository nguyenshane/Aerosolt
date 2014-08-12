using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

	// Use this for initialization

	GameObject player;

	void Start () {
		player = GameObject.Find ("First Person Controller");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(player.transform.position, transform.position) <= 4) {
			if(player.GetComponentInChildren<Player>().canOpenDoor == true) {


				iTween.MoveTo(gameObject, iTween.Hash("y", -4));
			
			}
		
		}
	}
}
