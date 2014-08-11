using UnityEngine;
using System.Collections;

/*
 * Contains stats of the cloud object
 * Handles stat changes (damage taken, etc)
 */

public class Cloud : MonoBehaviour {

	public float baseDamage = 10.0f; //Damage per second during contact, multiplied by remaining hp
	public float hitpoints = 200.0f;

	Color initialColor;
	Color currentColor;
	float hp;
	float damage;

	// Use this for initialization
	void Start () {
		initialColor = renderer.material.GetColor("_Color");
		currentColor = initialColor;
		hp = hitpoints;
		damage = baseDamage;
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


	public void recieveDamage(float incomingDamage, float scale) {
		hp -= incomingDamage * Mathf.Max(scale / transform.localScale.x, 1.0f);
		currentColor.a = initialColor.a * (hp / hitpoints);
		renderer.material.SetColor("_Color", currentColor);
		damage = baseDamage * currentColor.a;
		if (hp <= 0) Destroy(gameObject);
	}
}
