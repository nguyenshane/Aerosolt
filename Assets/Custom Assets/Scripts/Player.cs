using UnityEngine;
using System.Collections;

/*
 * Fires projectiles at the moment
 */

public class Player : MonoBehaviour {

	public Transform projectile;
	public Transform nozzle;
	public float fireDelay = 0.1f;
	public float projectileSpeed = 10.0f;
	protected Animator animator;

	public CharacterController character;
	float fireDelayTimer;


	// Use this for initialization
	void Start () {
		if(!character)
		character = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		fireDelayTimer = 0;	
	}
	
	// Update is called once per frame
	void Update () {
		if (fireDelayTimer >= 0) fireDelayTimer -= Time.deltaTime;

		if(animator)
		{
			//get the current state
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


		}

		if (Input.GetAxis("Aim") > 0){
			animator.SetBool("Aim", true );
		if (Input.GetAxis("Fire1") > 0 && fireDelayTimer <= 0) {
			fireDelayTimer = fireDelay;
			Gamepad.SetVibration(0,testA,testB);
			Projectile newProjectile = ((Transform)Instantiate(projectile, nozzle.position + (transform.forward * 0.5f), Quaternion.identity)).gameObject.GetComponent<Projectile>();
			newProjectile.rigidbody.velocity = character.velocity + transform.forward * projectileSpeed;
		}
		}
		else animator.SetBool("Aim", false );
	}
}