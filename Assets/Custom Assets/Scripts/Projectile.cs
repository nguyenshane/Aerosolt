using UnityEngine;
using System.Collections;

/*
 * Basic projectile for prototype purposes only
 */

public class Projectile : MonoBehaviour {

	const float disperseRate = 2.0f;
	const float gravity = 8.0f;
	const float drag = 0.5f;

	public Vector3 velocity;

	Color initialColor;
	Color currentColor;

	// Use this for initialization
	void Start () {
		initialColor = renderer.material.GetColor("_TintColor");
		currentColor = initialColor;
	}
	
	// Update is called once per frame
	void Update () {
		//Manual physics
		velocity.y -= gravity * Time.deltaTime;
		velocity -= (velocity * drag * Time.deltaTime);

		transform.Translate(velocity * Time.deltaTime);

		//Changing transparency
		currentColor.a -= (currentColor.a * disperseRate * Time.deltaTime);
		renderer.material.SetColor("_TintColor", currentColor);
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
}