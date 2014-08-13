using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {

	public float pickupDistance = 3.0f;

	GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("First Person Controller");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(player.transform.position, transform.position) <= pickupDistance) {
			player.GetComponentInChildren<Player>().hasKey = true;
			Destroy(gameObject);
		}
	}
}
