using UnityEngine;
using System.Collections;

/*
 * Contains stats of the enemy object
 * Handles stat changes (damage taken, etc)
 */

public class Enemy : MonoBehaviour {

	public float damage = 25.0f; //Damage per second during contact
	public float hitpoints = 100.0f;
	public Vector2 speedRange = new Vector2(4.0f, 6.0f);
	public Vector2 accelerationRange = new Vector2(2.0f, 5.0f);

	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float acceleration;

	GameObject parent;
	Color initialColor;
	Color currentColor;
	float hp;


	// Use this for initialization
	void Start () {
		parent = transform.parent.gameObject;
		initialColor = parent.renderer.material.GetColor("_Color");
		currentColor = initialColor;
		hp = hitpoints;

		speed = Random.Range(speedRange.x, speedRange.y);
		acceleration = Random.Range(accelerationRange.x, accelerationRange.y);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider collection) {
		string tag = collection.gameObject.tag;
		
		switch (tag) {
		case "Player":
			Player.recieveDamage(damage * Time.deltaTime);
			break;

		default:
			break;
		}
	}

	public void recieveDamage(float damage, float scale) {
		hp -= damage * Mathf.Max(scale / transform.localScale.x, 1.0f);
		currentColor.r = initialColor.r * (hp / hitpoints);
		currentColor.g = initialColor.g * (hp / hitpoints);
		currentColor.b = initialColor.b * (hp / hitpoints);
		parent.renderer.material.SetColor("_Color", currentColor);
		if (hp <= 0) Destroy(parent);
	}
}
