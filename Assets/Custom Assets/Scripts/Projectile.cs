﻿using UnityEngine;
using System.Collections;

/*
 * Basic projectile for prototype purposes only
 */

public class Projectile : MonoBehaviour {

	const float baseDamage = 1.0f;
	const float disperseRate = 4.0f;
	const float scaleRate = 2.5f; //size increase over time
	const float gravity = 4.0f;
	const float drag = 1.0f;

	public Vector3 velocity;

	Color initialColor;
	Color currentColor; //used to change alpha, also determines damage amount
	float damage; //actual damage done is (damage * (max(scale / target scale, 1)), should be revised to keep each projectile's damage exactly the same over time
	float scale;

	// Use this for initialization
	void Start () {
		initialColor = renderer.material.GetColor("_TintColor");
		currentColor = initialColor;
		damage = baseDamage;
		scale = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		float time = Time.deltaTime;

		//Manual physics
		velocity.y -= gravity * time;
		velocity -= (velocity * drag * time);

		transform.Translate(velocity * time);

		//Changing size
		transform.localScale += (transform.localScale * scaleRate * time);
		scale = transform.localScale.x;

		//Changing transparency and damage
		currentColor.a -= (currentColor.a * disperseRate * time);
		renderer.material.SetColor("_TintColor", currentColor);
		damage = baseDamage * currentColor.a;
		if (currentColor.a <= 0.01f) Destroy(gameObject);
	}

	/*
	void OnCollisionEnter(Collision Collection) {
		string tag = Collection.gameObject.tag;

		switch (tag) {
		case "Enemy":
			Destroy(Collection.gameObject);
			Destroy(gameObject);
			break;

		case "Environment":
			Destroy(gameObject);
			break;

		default:
			//Destroy(gameObject);
			break;
		}
	}
	*/

	void OnTriggerEnter(Collider Collection) {
		string tag = Collection.gameObject.tag;
		
		switch (tag) {
		case "Enemy":
			Collection.gameObject.GetComponent<Enemy>().recieveDamage(damage, scale);
			Destroy(gameObject);
			break;
			
		case "Environment":
			Destroy(gameObject);
			break;
			
		default:
			//Destroy(gameObject);
			break;
		}
	}
}