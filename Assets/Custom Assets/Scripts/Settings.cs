using UnityEngine;
using System.Collections;
using UnityEngine;
using System.Collections;

/*
 * Transfers audio volume settings to this object
 */

public class Settings : MonoBehaviour {

	// Use this for initialization
	void Start () {
		audio.volume = OptionController.bgmvolume;
		AudioListener.volume = OptionController.sfxvolume;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
