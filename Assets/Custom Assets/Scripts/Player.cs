using UnityEngine;
using System.Collections;

/*
 * Fires projectiles at the moment
 */

public class Player : MonoBehaviour {

	public Transform projectile;
	public Transform nozzle;
	public float fireDelay = 0.1f;
	protected Animator animator;

	public CharacterController character;
	public float fireRate = 120.0f; //projectiles per second
	public float projectileSpeed = 12.0f;
	public float spread = 0.3f;

	const float velocityDeviation = 0.2f;

	float fireDelayTimer;


	// Use this for initialization
	void Start () {
		if(!character)
		character = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		fireDelayTimer = 0;	
		fireDelay = 1.0f / fireRate;
	}
	
	// Update is called once per frame
	void Update () {
		if (fireDelayTimer >= 0) fireDelayTimer -= Time.deltaTime;
		else if (Input.GetAxis("Aim") > 0){
			animator.SetBool("Aim", true );
		
		if (Input.GetAxis("Fire1") > 0) {
			while (fireDelayTimer < fireDelay) {
				fireDelayTimer += fireDelay; //allows the correct number of projectiles to be fired per second regardless of framerate


		
		if (Input.GetAxis("Fire1") > 0 && fireDelayTimer <= 0) {
			fireDelayTimer = fireDelay;
			Projectile newProjectile = ((Transform)Instantiate(projectile, nozzle.position + (transform.forward * 0.5f), Quaternion.identity)).gameObject.GetComponent<Projectile>();
			Vector3 direction = transform.forward + (Random.onUnitSphere * spread);
			direction.Normalize();
			newProjectile.rigidbody.velocity = character.velocity + (direction * projectileSpeed * (1 + Random.Range(-velocityDeviation, velocityDeviation)));
			}
		  }
		}
	}
		else animator.SetBool("Aim", false );
  }
}