using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public float hitpoints = 250.0f;
	
	Color initialColor;
	Color currentColor;
	float hp;

	// Use this for initialization
	void Start () {
		initialColor = renderer.material.GetColor("_Color");
		currentColor = initialColor;
		hp = hitpoints;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void recieveDamage(float incomingDamage, float scale) {
		hp -= incomingDamage * Mathf.Max(scale / transform.localScale.x, 1.0f);
		currentColor.r = initialColor.r * (hp / hitpoints);
		currentColor.g = initialColor.g * (hp / hitpoints);
		currentColor.b = initialColor.b * (hp / hitpoints);
		renderer.material.SetColor("_Color", currentColor);
		if (hp <= 0) Destroy(gameObject);
	}
}
