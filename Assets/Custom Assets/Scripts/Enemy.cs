using UnityEngine;
using System.Collections;

/*
 * Contains stats of the enemy object
 * Handles stat changes (damage taken, etc)
 */

public class Enemy : MonoBehaviour {

	public float hitpoints = 100.0f;
	public Vector2 speedRange = new Vector2(4.0f, 6.0f);
	public Vector2 accelerationRange = new Vector2(2.0f, 5.0f);

	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float acceleration;

	// Use this for initialization
	void Start () {
		speed = Random.Range(speedRange.x, speedRange.y);
		acceleration = Random.Range(accelerationRange.x, accelerationRange.y);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void recieveDamage(float damage, float scale) {
		hitpoints -= damage * Mathf.Max(scale / transform.localScale.x, 1.0f);
		if (hitpoints <= 0) Destroy(gameObject);
	}
}
