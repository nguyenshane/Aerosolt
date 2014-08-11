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
		playerScript = player.GetComponentInChildren<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(player.transform.position, transform.position) <= pickupDistance) {
			float ammoDifference = playerScript.ammunition - playerScript.getAmmo();
			if (ammoDifference >= amount) {
				playerScript.addAmmo(amount);
				Destroy(gameObject);
			} else {
				playerScript.addAmmo(ammoDifference);
				amount -= ammoDifference;
			}
		}
	}

	void OnCollisionEnter(Collision collection) {
		string tag = collection.gameObject.tag;
		
		switch (tag) {
		case "Player":
			Player player = collection.gameObject.GetComponentInChildren<Player>();
			float ammoDifference = player.ammunition - player.getAmmo();
			if (ammoDifference >= amount) {
				player.addAmmo(amount);
				Destroy(gameObject);
			} else {
				player.addAmmo(ammoDifference);
				amount -= ammoDifference;
			}
			break;
			
		default:
			break;
		}
	}
}
