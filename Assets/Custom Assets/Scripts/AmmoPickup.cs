using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour {

	public float amount = 25.0f;
	public float pickupDistance = 4.0f;

	GameObject player;
	Player playerScript;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("First Person Controller");
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(player.transform.position, transform.position) <= pickupDistance) {
			float ammoDifference = Player.ammunition - Player.getAmmo();
			if (ammoDifference >= amount) {
				Player.addAmmo(amount);
				Destroy(gameObject);
			} else {
				Player.addAmmo(ammoDifference);
				amount -= ammoDifference;
			}
		}
	}

	void OnCollisionEnter(Collision collection) {
		string tag = collection.gameObject.tag;
		
		switch (tag) {
		case "Player":
			float ammoDifference = Player.ammunition - Player.getAmmo();
			if (ammoDifference >= amount) {
				Player.addAmmo(amount);
				Destroy(gameObject);
			} else {
				Player.addAmmo(ammoDifference);
				amount -= ammoDifference;
			}
			break;
			
		default:
			break;
		}
	}
}
