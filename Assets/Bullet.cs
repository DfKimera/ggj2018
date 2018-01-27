using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public class Bullet : MonoBehaviour {
	
	const float SPEED = 50f;
	private const int MAX_LIFETIME = 100;

	public TeamID team;
	public int lifetime = 0;

	public void Fire(Vector3 start, float aimX, float aimY, TeamID team) {
		this.team = team;

		GetComponent<Rigidbody>().position = start;
		GetComponent<Rigidbody>().velocity = new Vector3(aimX * SPEED, 0, aimY * SPEED);
		
		GetComponent<TrailRenderer>().enabled = true;
	}

	public void Update() {
		lifetime++;
		
		if (lifetime > MAX_LIFETIME) {
			Destroy(gameObject);
		}
	}

	public void OnCollisionEnter(Collision collision) {
		Debug.Log("HIT: " + collision.gameObject);
		
		if (collision.gameObject.CompareTag("Player") && team == TeamID.Player) {
			return;
		}
		
		//Destroy(gameObject);
	}
}
