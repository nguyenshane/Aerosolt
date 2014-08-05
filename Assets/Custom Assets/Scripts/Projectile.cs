using UnityEngine;
using System.Collections;

/*
 * Basic projectile for prototype purposes only
 */

public class Projectile : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
			Destroy(gameObject);
			break;
		}
	}
}