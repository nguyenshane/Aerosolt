using UnityEngine;
using System.Collections;

/*
 * Fires projectiles at the moment
 */

public class Player : MonoBehaviour {

	public Transform projectile;
	public Transform nozzle;
	public float fireRate = 120.0f; //projectiles per second
	public float projectileSpeed = 12.0f;
	public float spread = 0.3f;

	const float velocityDeviation = 0.2f;

	protected Animator animator;
	CharacterController character;
	float fireDelay;
	float fireDelayTimer;


	// Use this for initialization
	void Start () {
		character = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		
		fireDelay = 1.0f / fireRate;
		fireDelayTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (fireDelayTimer >= 0) fireDelayTimer -= Time.deltaTime;
		else if (Input.GetAxis("Fire1") > 0) {
			while (fireDelayTimer < fireDelay) {
				fireDelayTimer += fireDelay; //allows the correct number of projectiles to be fired per second regardless of framerate

				Vector3 direction = transform.forward + (Random.onUnitSphere * spread);
				direction.Normalize();

				Projectile newProjectile = ((Transform)Instantiate(projectile, nozzle.position + (transform.forward * 0.5f), Quaternion.identity)).gameObject.GetComponent<Projectile>();
				newProjectile.rigidbody.velocity = character.velocity + (direction * projectileSpeed * (1 + Random.Range(-velocityDeviation, velocityDeviation)));
			}
		}

		if (animator) {
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (Input.GetAxis("Aim") > 0) animator.SetBool("Aim", true);
			else animator.SetBool("Aim", false);
		}
	}
}