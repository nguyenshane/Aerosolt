using UnityEngine;
using System.Collections;

/*
 * Basic projectile for prototype purposes only
 */

public class Projectile : MonoBehaviour {

	const float disperseRate = 4.0f;
	const float scaleRate = 2.5f;
	const float gravity = 8.0f;
	const float drag = 1.0f;

	public Vector3 velocity;

	Color initialColor;
	Color currentColor; //used to change alpha, also determines damage amount later

	// Use this for initialization
	void Start () {
		initialColor = renderer.material.GetColor("_TintColor");
		currentColor = initialColor;
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

		//Changing transparency
		currentColor.a -= (currentColor.a * disperseRate * time);
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