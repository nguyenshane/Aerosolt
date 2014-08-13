using UnityEngine;
using System.Collections;

/*
 * Waypoint for AI pathfinding
 */

public class Waypoint : MonoBehaviour {

	public GameObject[] adjacent; //connected waypoints in each of the 4 directions
	public bool[] blocked; //true if there is a destructible obstacle in the way of this adjacent waypoint that needs to be checked

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}
}
