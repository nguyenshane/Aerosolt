using UnityEngine;
using System.Collections;

/*
 * Handles level progression
 * Attached to the door on each level
 */

public class LevelCompletion : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collection) {
		if (collection.gameObject.tag == "Player" &&
		    GameObject.FindGameObjectWithTag("Enemy") == null &&
		    collection.gameObject.GetComponentInChildren<Player>().hasKey) {
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}
}
