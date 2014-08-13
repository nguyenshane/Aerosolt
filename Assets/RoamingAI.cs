using UnityEngine;
using System.Collections;

/*
 * Handles roaming AI behavior and movement
 */

public class RoamingAI : MonoBehaviour {

	public float targetAcquisitionTime = 0.5f;
	public float reactionTime = 0.2f;

	static float waypointHeight = 3.0f;

	float targetAcquisitionTimer;
	float reactionTimer;
	bool hasTarget = false;
	bool returning = true;
	Vector3 targetDir;
	RaycastHit hitinfo;
	int layerMask, sightMask;

	GameObject currentWaypoint;
	bool insideWaypoint = false;
	Transform target;
	GameObject nextWaypoint;
	Enemy stats;

	GameObject[] adjacentWaypoints;
	
	// Use this for initialization
	void Start () {
		target = GameObject.Find("First Person Controller").transform;
		stats = GetComponentInChildren<Enemy>();
		hitinfo = new RaycastHit();
		
		targetAcquisitionTimer = targetAcquisitionTime;
		reactionTimer = reactionTime;

		layerMask = (1 << 9) | (1 << 11);
		sightMask = (1 << 10) | (1 << 11);
		sightMask = ~sightMask;

		findNextWaypoint();
	}
	
	// Update is called once per frame
	void Update () {
		if (targetAcquisitionTimer >= 0) targetAcquisitionTimer -= Time.deltaTime;
		else {
			targetAcquisitionTimer = Random.Range(targetAcquisitionTime / 2, targetAcquisitionTime * 1.5f);
			targetDir = target.position - transform.position + (Random.insideUnitSphere * target.collider.bounds.size.x * 0.7f);
			Physics.Raycast(transform.position, targetDir, out hitinfo, Mathf.Infinity, sightMask);
			
			if (hitinfo.collider != null) {
				if (hitinfo.collider.tag == "Player") {
					//Follow target
					hasTarget = true;
					returning = false;
				} else {
					//Mirror the previous raycast across the vector pointing to the center of the player and try again
					targetDir = target.position - transform.position - new Vector3(targetDir.x - transform.position.x - target.position.x, 0, targetDir.z - transform.position.z - target.position.z);
					Physics.Raycast(transform.position, targetDir, out hitinfo, Mathf.Infinity, sightMask);
					
					if (hitinfo.collider.tag == "Player") {
						//Follow target
						hasTarget = true;
						returning = false;
					} else {
						hasTarget = false;
						
						if (hitinfo.distance <= transform.localScale.x + Random.Range(0.2f, 0.7f) + Mathf.Pow(rigidbody.velocity.magnitude, 2) / (2 * stats.acceleration)) {
							//About to run into something that isn't the player, recalculate roaming path
							hasTarget = false;
							returning = true;
							findNextWaypoint();
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
			//Continue roaming
			if (nextWaypoint != null) {
				Vector3 wpDirection = (nextWaypoint.transform.position - transform.position + (Random.insideUnitSphere * 2.0f));
				Debug.DrawRay(transform.position, wpDirection, Color.green);

				if (wpDirection.magnitude <= 2.0f) {
					findNextWaypoint();
					wpDirection = (nextWaypoint.transform.position - transform.position + (Random.insideUnitSphere * 2));
				}
				wpDirection.Normalize();

				rigidbody.AddForce(wpDirection * stats.acceleration * Time.deltaTime, ForceMode.Impulse);
			} //else Debug.Log("Waypoint not found");
		}
		
		if (rigidbody.velocity.magnitude > stats.speed) rigidbody.velocity = (rigidbody.velocity.normalized * stats.speed);

		insideWaypoint = false;
	}

	void OnTriggerStay(Collider collection) {
		if (collection.tag == "Waypoint") {
			insideWaypoint = true;
			currentWaypoint = collection.gameObject;
		}
	}


	private void findNextWaypoint() {
		getNearestWaypoints();

		int[] waypointPicker = {0, 1, 2, 3};

		for (int i = 3; i >= 0; i--) {
			int k = Random.Range(0, i + 1);
			int value = waypointPicker[k];
			waypointPicker[k] = waypointPicker[i];
			waypointPicker[i] = value;
		}

		nextWaypoint = null;

		for (int i = 0; i < 4; i++) {
			Debug.Log(adjacentWaypoints[waypointPicker[i]]);
			if (adjacentWaypoints[waypointPicker[i]] != null) {
				nextWaypoint = adjacentWaypoints[i];
				break;
			}
		}
	}

	private void getNearestWaypoints() {
		if (insideWaypoint) {
			adjacentWaypoints = currentWaypoint.GetComponent<Waypoint>().adjacent;
		} else {
			adjacentWaypoints = new GameObject[4];

			Physics.Raycast(new Vector3(transform.position.x, waypointHeight, transform.position.z), Vector3.left, out hitinfo, Mathf.Infinity, layerMask);
			if (hitinfo.collider.tag == "Waypoint") Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.left * hitinfo.distance, Color.red, 1.0f);
			else Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.left * hitinfo.distance, Color.blue, 1.0f);
			if (hitinfo.collider.tag == "Waypoint") adjacentWaypoints[0] = hitinfo.collider.gameObject;

			Physics.Raycast(new Vector3(transform.position.x, waypointHeight, transform.position.z), Vector3.right, out hitinfo, Mathf.Infinity, layerMask);
			if (hitinfo.collider.tag == "Waypoint") Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.right * hitinfo.distance, Color.red, 1.0f);
			else Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.right * hitinfo.distance, Color.blue, 1.0f);
			if (hitinfo.collider.tag == "Waypoint") adjacentWaypoints[1] = hitinfo.collider.gameObject;

			Physics.Raycast(new Vector3(transform.position.x, waypointHeight, transform.position.z), Vector3.forward, out hitinfo, Mathf.Infinity, layerMask);
			if (hitinfo.collider.tag == "Waypoint") Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.forward * hitinfo.distance, Color.red, 1.0f);
			else Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.forward * hitinfo.distance, Color.blue, 1.0f);
			if (hitinfo.collider.tag == "Waypoint") adjacentWaypoints[2] = hitinfo.collider.gameObject;

			Physics.Raycast(new Vector3(transform.position.x, waypointHeight, transform.position.z), Vector3.back, out hitinfo, Mathf.Infinity, layerMask);
			if (hitinfo.collider.tag == "Waypoint") Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.back * hitinfo.distance, Color.red, 1.0f);
			else Debug.DrawRay (new Vector3 (transform.position.x, waypointHeight, transform.position.z), Vector3.back * hitinfo.distance, Color.blue, 1.0f);
			if (hitinfo.collider.tag == "Waypoint") adjacentWaypoints[3] = hitinfo.collider.gameObject;
		}
	}
}
