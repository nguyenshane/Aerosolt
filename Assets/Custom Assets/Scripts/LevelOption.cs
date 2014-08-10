using UnityEngine;
using System.Collections;

public class LevelOption : MonoBehaviour {

	// Use this for initialization
	void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {

		// Audio volumes
		audio.volume = OptionController.bgmvolume;
		AudioListener.volume = OptionController.sfxvolume;
	
	}
}
