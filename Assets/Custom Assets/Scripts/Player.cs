using UnityEngine;
using System.Collections;

/*
 * Fires projectiles
 * Handles aiming animations
 */

public class Player : MonoBehaviour {

	public Transform projectile;
	public Transform nozzle;
	public float hitpoints = 100.0f;
	public float fireRate = 120.0f; //projectiles per second
	public float projectileSpeed = 12.0f;
	public float spread = 0.3f;

	const float velocityDeviation = 0.2f;

	protected Animator animator;
	public CharacterController character;
	float fireDelay;
	float fireDelayTimer;
	float hp;


	// Use this for initialization
	void Start () {
		if(!character)
		character = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();

		hp = hitpoints;
		fireDelay = 1.0f / fireRate;
		fireDelayTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (fireDelayTimer >= 0) fireDelayTimer -= Time.deltaTime;
		else if (Input.GetAxis ("Fire1") > 0) {
			while (fireDelayTimer < fireDelay) {
				fireDelayTimer += fireDelay; //allows the correct number of projectiles to be fired per second regardless of framerate
				
				Vector3 direction = transform.forward + (Random.onUnitSphere * spread);
				direction.Normalize ();
				
				Projectile newProjectile = ((Transform)Instantiate (projectile, nozzle.position + (transform.forward * 0.2f), Quaternion.identity)).gameObject.GetComponent<Projectile> ();
				newProjectile.velocity = character.velocity + (direction * projectileSpeed * (1 + Random.Range (-velocityDeviation, velocityDeviation)));
			}
		} 

		if (Input.GetAxis ("Aim") > 0) animator.SetBool ("Aim", true);
		else animator.SetBool("Aim", false);


		/*
		if (animator) {
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (Input.GetAxis("Aim") > 0) animator.SetBool("Aim", true);
			else animator.SetBool("Aim", false);
		}
		*/
	}

	public void recieveDamage(float damage) {
		hp -= damage;
		/*
		currentColor.r = initialColor.r * (hp / hitpoints);
		currentColor.g = initialColor.g * (hp / hitpoints);
		currentColor.b = initialColor.b * (hp / hitpoints);
		renderer.material.SetColor("_Color", currentColor);
		*/
		//if (hp <= 0) Destroy(gameObject);
	}

	public float getHP() { return hp; }
}