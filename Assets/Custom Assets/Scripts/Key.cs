using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {

	public float pickupDistance = 3.0f;
	public float level = 0;

	GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("First Person Controller");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(player.transform.position, transform.position) <= pickupDistance) {
			if (level == 1) {
				player.GetComponentInChildren<Player>().hasKey[Application.loadedLevel] = true;
			} else if (level == 2) {
				player.GetComponentInChildren<Player>().canOpenDoor = true;
			}
			Destroy(gameObject);
			
		}
	}
}
