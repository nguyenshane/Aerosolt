using UnityEngine;
using System.Collections;

/*
 * Fires projectiles at the moment
 */

public class Player : MonoBehaviour {

	public Transform projectile;
	public Transform spawn;
	public float fireDelay = 1.0f;
	public float projectileSpeed = 6.0f;

	float fireDelayTimer;


	// Use this for initialization
	void Start () {
		fireDelayTimer = 0;	
	}
	
	// Update is called once per frame
	void Update () {
		if (fireDelayTimer >= 0) fireDelayTimer -= Time.deltaTime;

		if (Input.GetAxis("Fire1") > 0 && fireDelayTimer <= 0) {
			fireDelayTimer = fireDelay;
			//Transform spawn = transform.FindChild("SquirtBottle").FindChild("Nozzle").FindChild("Tip");

			Projectile newProjectile = ((Transform)Instantiate(projectile, spawn.position + (transform.forward * 0.5f), Quaternion.identity)).gameObject.GetComponent<Projectile>();
			newProjectile.rigidbody.velocity = transform.forward * projectileSpeed;
		}
	}
}