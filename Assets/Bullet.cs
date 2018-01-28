using System.Collections;
using System.Collections.Generic;
using Structs;
using UnityEngine;

public class Bullet : MonoBehaviour {
	
	public float speed = 50f;
	public int maxLifetime = 100;

	public TeamID teamID;
	public int lifetime = 0;
	public bool bounce = false;

	public void Fire(Vector3 start, float aimX, float aimY, TeamID team) {
		teamID = team;

		GetComponent<Rigidbody>().position = start;
		GetComponent<Rigidbody>().velocity = (new Vector3(aimX, 0, aimY)).normalized * speed;
		
		GetComponent<TrailRenderer>().enabled = true;
	}

	public void Update() {
		lifetime++;
		
		if (lifetime > maxLifetime) {
			Destroy(gameObject);
		}
	}

	public void OnCollisionEnter(Collision collision) {
		
		if (collision.gameObject.CompareTag("Player") && teamID == TeamID.Player) return;
		if (collision.gameObject.CompareTag("Enemies") && teamID == TeamID.Enemy) return;
		
		if(!bounce) Destroy(gameObject);
	}
}
