using UnityEngine;
using System.Collections;

public class OptionController : MonoBehaviour {
	// global variables for game settings
	public static float bgmvolume = 0.5f;
	public static float sfxvolume = 0.5f;
	public static float brightness = 1.0f;
	public static float sensitivity = 1.0f;
	float brightnesslight;
	// Use this for initialization
	void Start () {
		brightnesslight = GameObject.Find("Brightness Light").light.intensity;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject.Find("Brightness Light").light.intensity = brightness * brightnesslight;

		// Audio volumes
		audio.volume = bgmvolume;
		AudioListener.volume = sfxvolume;
	}


	public void BGM(string x){
	var mid = GameObject.Find("BGM Line").transform.position.x;
		//if(x == "mute") {bgmvolume = 0.0f; iTween.MoveTo(GameObject.Find("BGM Slider"),iTween.Hash ("x",mid-5,"time",1));}
		if(x == "minus" && bgmvolume >= 0.1f) {
			bgmvolume -= 0.1f;
			GameObject.Find("BGM Slider").transform.Translate(Vector3.right * 0.5f);
		}
		if(x == "plus" && bgmvolume <= 0.9f) {
			bgmvolume += 0.1f; 
			GameObject.Find("BGM Slider").transform.Translate(Vector3.left * 0.5f);
		}
			//iTween.MoveTo(GameObject.Find("BGM Slider"), iTween.Hash("x",mid-5+bgmvolume*10,"time",0));}
		audio.Play();
		Debug.Log(bgmvolume);
	}

	public void SFX(string x){
		var mid = GameObject.Find("SFX Line").transform.position.x;
		//if(x == "mute") {bgmvolume = 0.0f; iTween.MoveTo(GameObject.Find("BGM Slider"),iTween.Hash ("x",mid-5,"time",1));}
		if(x == "minus" && sfxvolume >= 0.1f) {
			sfxvolume -= 0.1f;
			GameObject.Find("SFX Slider").transform.Translate(Vector3.right * 0.5f);
		}
		if(x == "plus" && sfxvolume <= 0.9f) {
			sfxvolume += 0.1f; 
			GameObject.Find("SFX Slider").transform.Translate(Vector3.left * 0.5f);
		}
		//iTween.MoveTo(GameObject.Find("BGM Slider"), iTween.Hash("x",mid-5+bgmvolume*10,"time",0));}
		audio.Play();
		Debug.Log(sfxvolume);
	}
}