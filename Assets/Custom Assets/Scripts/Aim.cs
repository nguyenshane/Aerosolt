using UnityEngine;
using System.Collections;

public class Aim : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetAxis("Fire1") >= 1){
			animation["Aim"].wrapMode = WrapMode.ClampForever;
			animation.Play("Aim");
		} else if (Input.GetAxis("Fire1") <= 0){
			animation["Aim"].wrapMode = WrapMode.Once;
			//animation.Play("UnAim");
		}
	}
}
