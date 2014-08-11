using UnityEngine;
using System.Collections;

public class OptionController : MonoBehaviour {

	// global variables for game settings
	public static float bgmvolume = 0.5f;
	public static float sfxvolume = 0.5f;
	public static float brightness = 0.1f;
	public static float sensitivity = 10.0f;
	
	float brightnesslight;

	
	// Use this for initialization
	void Start () {
		// Get the initial intensity, then update this number
		brightnesslight = GameObject.Find("Brightness Light").light.intensity;
	}

	
	public void BGM(string x){
	var mid = GameObject.Find("BGM Line").transform.position.x;
		if(x == "minus" && bgmvolume >= 0.0f) {
			bgmvolume -= 0.1f;
			GameObject.Find("BGM Slider").transform.Translate(Vector3.right * 0.5f);
		}
		if(x == "plus" && bgmvolume <= 0.9f) {
			bgmvolume += 0.1f; 
			GameObject.Find("BGM Slider").transform.Translate(Vector3.left * 0.5f);
		}

		//iTween.MoveTo(GameObject.Find("BGM Slider"), iTween.Hash("x",mid-5+bgmvolume*10,"time",0));}
		GameObject.Find("First Person Controller").audio.volume = bgmvolume;
		audio.Play();
		Debug.Log(bgmvolume);
	}

	public void SFX(string x){
		var mid = GameObject.Find("SFX Line").transform.position.x;
		if(x == "minus" && sfxvolume >= 0.1f) {
			sfxvolume -= 0.1f;
			GameObject.Find("SFX Slider").transform.Translate(Vector3.right * 0.5f);
		}
		if(x == "plus" && sfxvolume <= 0.9f) {
			sfxvolume += 0.1f; 
			GameObject.Find("SFX Slider").transform.Translate(Vector3.left * 0.5f);
		}
		audio.Play();
		AudioListener.volume = sfxvolume;
		Debug.Log(sfxvolume);
	}

	public void Bright(string x){
		var mid = GameObject.Find("Bright Line").transform.position.x;
		if(x == "minus" && brightness >= 0.01f) {
			brightness -= 0.01f;
			GameObject.Find("Bright Slider").transform.Translate(Vector3.right * 0.2f);
		}
		if(x == "plus" && brightness <= 0.19f) {
			brightness += 0.01f;
			GameObject.Find("Bright Slider").transform.Translate(Vector3.left * 0.2f);
		}
		audio.Play();
		GameObject.Find("Brightness Light").light.intensity = brightness  * 1;
		Debug.Log(brightness);
	}

	public void Sen(string x){
		var mid = GameObject.Find("Sen Line").transform.position.x/2;
		if(x == "minus" && sensitivity >= 7f) {
			sensitivity -= 0.1f * 7;
			GameObject.Find("Sen Slider").transform.Translate(Vector3.right * 0.5f);
		}
		if(x == "plus" && sensitivity <= 12.5f) {
			sensitivity += 0.1f * 7; 
			GameObject.Find("Sen Slider").transform.Translate(Vector3.left * 0.5f);
		}
		audio.Play();

		GameObject.Find("First Person Controller").GetComponent<MouseLook>().sensitivityX = sensitivity;

		Debug.Log(sensitivity);
	}
	
}