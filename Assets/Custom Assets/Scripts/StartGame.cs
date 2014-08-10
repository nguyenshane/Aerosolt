using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Application.LoadLevel(Application.loadedLevel+1);
	}
}
