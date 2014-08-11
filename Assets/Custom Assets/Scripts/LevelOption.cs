// This file should be attach to the object that carries the BGM

using UnityEngine;
using System.Collections;

public class LevelOption : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// Audio volumes
		audio.volume = OptionController.bgmvolume;
		AudioListener.volume = OptionController.sfxvolume;
		
		// Brightness
		GameObject.Find("Brightness Light").light.intensity = OptionController.brightness * 1;

		// Sensitivity
		GameObject.Find("First Person Controller").GetComponent<MouseLook>().sensitivityX = OptionController.sensitivity;

	}

}
