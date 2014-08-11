using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			Player.addAmmo(Player.ammunition - Player.getAmmo());
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}
}
