using UnityEngine;
using System.Collections;

/*
 * Handles AI behavior and movement
 */

public class AI : MonoBehaviour {

	public float targetAcquisitionTime = 0.5f;
	public float reactionTime = 0.2f;

	float targetAcquisitionTimer;
	float reactionTimer;
	bool hasTarget = false;
	bool returning = true;
	Vector3 returnPosition;
	Vector3 targetDir;
	RaycastHit hitinfo;

	Transform target;
	Enemy stats;

	// Use this for initialization
	void Start () {
		target = GameObject.Find("First Person Controller").transform;
		stats = GetComponent<Enemy>();
		hitinfo = new RaycastHit();

		targetAcquisitionTimer = targetAcquisitionTime;
		reactionTimer = reactionTime;
		returnPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (targetAcquisitionTimer >= 0) targetAcquisitionTimer -= Time.deltaTime;
		else {
			targetAcquisitionTimer = Random.Range(targetAcquisitionTime / 2, targetAcquisitionTime * 1.5f);
			targetDir = target.position - transform.position + (Random.insideUnitSphere * target.collider.bounds.size.x * 0.7f);
			Physics.Raycast(transform.position, targetDir, out hitinfo);

			if (hitinfo.collider != null) {
				if (hitinfo.collider.tag == "Player") {
					//Follow target
					hasTarget = true;
					returning = false;
				} else {
					//Mirror the previous raycast across the vector pointing to the center of the player and try again
					targetDir = target.position - transform.position - new Vector3(targetDir.x - transform.position.x - target.position.x, 0, targetDir.z - transform.position.z - target.position.z);
					Physics.Raycast(transform.position, targetDir, out hitinfo);

					if (hitinfo.collider.tag == "Player") {
						//Follow target
						hasTarget = true;
						returning = false;
					} else {
						hasTarget = false;

						if (hitinfo.distance <= transform.localScale.x + Random.Range(0.2f, 0.7f) + Mathf.Pow(rigidbody.velocity.magnitude, 2) / (2 * stats.acceleration)) {
							//About to run into something that isn't the player, begin returning
							hasTarget = false;
							returning = true;
						}
					}
				}
			}
		}

		if (reactionTimer >= 0) reactionTimer -= Time.deltaTime;
		else if (!returning && hasTarget) {
			reactionTimer = Random.Range(reactionTime / 2, reactionTime * 1.5f);
			targetDir = target.position - transform.position + (Random.insideUnitSphere * target.collider.bounds.size.x * 0.7f);
		}

		if (!returning) {
			//Either has line of sight or has just lost it, follow the previous raycast direction
			rigidbody.AddForce(targetDir.normalized * stats.acceleration * Time.deltaTime, ForceMode.Impulse);
		} else {
			//Return to original location
			rigidbody.AddForce((returnPosition - transform.position).normalized * stats.acceleration * Time.deltaTime, ForceMode.Impulse);
		}

		if (rigidbody.velocity.magnitude > stats.speed) rigidbody.velocity = (rigidbody.velocity.normalized * stats.speed);
	}
}
