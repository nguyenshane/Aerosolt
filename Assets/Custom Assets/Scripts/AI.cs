using UnityEngine;
using System.Collections;

/*
 * Handles AI behavior and movement
 */

public class AI : MonoBehaviour {

	Transform target;
	Enemy stats;

	// Use this for initialization
	void Start () {
		target = GameObject.Find("First Person Controller").transform;
		stats = GetComponent<Enemy>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetDir = target.position - transform.position;
		RaycastHit hitinfo = new RaycastHit();
		Physics.Raycast(transform.position, targetDir, out hitinfo);

		if (hitinfo.collider.tag == "Player") {
			//Follow target
			rigidbody.AddForce(targetDir.normalized * stats.acceleration, ForceMode.Impulse);
			if (rigidbody.velocity.magnitude > stats.speed) rigidbody.velocity = (rigidbody.velocity.normalized * stats.speed);
		}
	}
}
