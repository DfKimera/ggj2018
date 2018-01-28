using System.Collections;
using System.Collections.Generic;
using Entities;
using Structs;
using UnityEngine;

public class Bullet : MonoBehaviour {
	
	public float speed = 40f;
	public int maxLifetime = 100;

	public TeamID teamID;
	public int lifetime = 0;
	public int damage = 10;
	public bool bounce = false;
	
	public AudioClip[] sfxBulletHit;

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
	
	public void PlaySFX(AudioClip clip, float volume = 1.0f) {
		AudioSource.PlayClipAtPoint(clip, transform.position, volume);
	}

	public void OnCollisionEnter(Collision collision) {

		if (collision.gameObject.CompareTag("Player")) {
			collision.gameObject.GetComponent<Player>().ApplyDamage(damage);
			Destroy(gameObject);
			return;
		}

		if (collision.gameObject.CompareTag("Enemy")) {
			// TODO: apply damage
			Destroy(gameObject);
		};
		
		// TODO: meat hit sfx
		// TODO: particle emit
		
		AudioClip hitClip = sfxBulletHit[Random.Range(0, sfxBulletHit.Length - 1)];
		
		PlaySFX(hitClip);
		
		if(!bounce) Destroy(gameObject);
	}
}
