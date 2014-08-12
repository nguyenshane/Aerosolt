using UnityEngine;
using System.Collections;

/*
 * Fires projectiles
 * Handles aiming animations
 */

public class Player : MonoBehaviour {
	
	public static float hitpoints = 100.0f;
	public static float ammunition = 100.0f;
	public float burstRate = 3.0f; //trigger presses per second for constant fire
	public float fireRate = 960.0f; //projectiles per second
	public float projectileSpeed = 12.0f;
	public float spread = 0.3f;
	public float ammoConsumption = 0.01f;
	public Transform projectile;
	public Transform nozzle;

	public bool hasKey;

	const float velocityDeviation = 0.2f;
	const float deadzone = 0.1f;

	protected Animator animator;
	public CharacterController character;
	float fireDelay, fireDelayTimer, burstDelay, burstDelayTimer;
	bool firing, triggerReset;
	static float hp, ammo;
	static bool initialized = false;
	
	public static float getHP() { return hp; }
	public static float getAmmo() { return ammo; }

	// Use this for initialization
	void Start () {
		if(!character)
		character = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();

		if (!initialized) {
			hp = hitpoints;
			ammo = ammunition;
			initialized = true;
		}

		burstDelay = 1.0f / burstRate;
		burstDelayTimer = 0.0f;
		fireDelay = 1.0f / fireRate;
		fireDelayTimer = 0.0f;
		firing = triggerReset = false;

		hasKey = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (burstDelayTimer >= 0) burstDelayTimer -= Time.deltaTime;
		else firing = false;

		float fireInput = Input.GetAxis("Fire1");
		if (fireInput <= deadzone) triggerReset = true;

		if (burstDelayTimer <= 0 && !firing && ammo >= ammoConsumption) {
			if (triggerReset && fireInput > deadzone) {
				firing = true;
				triggerReset = false;
				burstDelayTimer = burstDelay;
				audio.Play();
			}
		}

		if (fireDelayTimer >= 0) fireDelayTimer -= Time.deltaTime;
		else if (firing) {
			while (fireDelayTimer < fireDelay && ammo >= ammoConsumption) {
				fireDelayTimer += fireDelay; //allows the correct number of projectiles to be fired per second regardless of framerate
				
				Vector3 direction = transform.forward + (Random.onUnitSphere * spread);
				direction.Normalize ();
				
				Projectile newProjectile = ((Transform)Instantiate (projectile, nozzle.position + (transform.forward * 0.2f), Quaternion.identity)).gameObject.GetComponent<Projectile> ();
				newProjectile.velocity = character.velocity + (direction * projectileSpeed * (1 + Random.Range (-velocityDeviation, velocityDeviation)));

				ammo -= ammoConsumption;
			}
		}

		if (Input.GetAxis ("Aim") > 0) animator.SetBool ("Aim", true);
		else animator.SetBool("Aim", false);
	}


	public static void recieveDamage(float damage) {
		hp -= damage;

		if (hp <= 0) {
			GameObject.Find("GUI Controller").GetComponent<GUIController>().activateDeathScreen();
			resetStats();
		}
	}

	public static void resetStats() {
		hp = hitpoints;
		ammo = ammunition;
	}

	public static void addAmmo(float amount) {
		ammo += amount;
	}
}