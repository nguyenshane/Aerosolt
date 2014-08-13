using UnityEngine;
using System.Collections;

public class CameraFacing : MonoBehaviour {

	Transform cameraTransform;

	// Use this for initialization
	void Start () {
		cameraTransform = GameObject.Find("OVRCameraController").gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(cameraTransform.position, Vector3.up);
	}
}
