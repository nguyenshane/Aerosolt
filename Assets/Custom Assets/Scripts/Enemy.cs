using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float hitpoints = 100.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void recieveDamage(float damage, float scale) {
		hitpoints -= damage * Mathf.Max(scale / transform.localScale.x, 1.0f);
		if (hitpoints <= 0) Destroy(gameObject);
	}
}
