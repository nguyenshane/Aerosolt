﻿using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}
}
