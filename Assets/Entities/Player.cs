﻿using System;
using InputSystem;
using Structs;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities{
	
	public class Player : MonoBehaviour {
		
		private const float SPEED_FORCE_MULTIPLIER = 250f;
		private const float MAX_AIRSPEED = 6f;

		private Rigidbody body;
		private SphereCollider radius;
		private CapsuleCollider collision;
		private Animator animator;
		private InputController ctrl;
	
		public PlayerID id = PlayerID.Player1;

		public PhysicMaterial noCollideMat;
		public PhysicMaterial doCollideMat;
		public GameObject bulletPrefab;

		public string animationName = "player_idle";

		public float moveX = 0;
		public float moveY = 0;
		
		public float aimX = 0;
		public float aimY = 0;
		
		public float speed = 150.0f;
		public float animationDeadzone = 0.05f;
		
		
		public int attackCooldown = 0;
		public int attackDelay = 10;
		public float attackForce = 85.0f;
		
		public int jumpCooldown = 0;
		public int jumpDelay = 20;
		public float jumpForce = 125f;
		
		public int footstepsCooldown = 0;
		public int footstepsDelay = 8;

		public AudioClip[] sfxFootsteps;
		public AudioClip sfxAttack;
		public AudioClip sfxStunned;
		public AudioClip sfxPickBlock;
		public AudioClip sfxDropBlock;
	
		protected void Start () {
			body = GetComponent<Rigidbody>();
			collision = GetComponent<CapsuleCollider>();
			animator = GetComponentInChildren<Animator>();
			ctrl = GetComponent<InputController>();
			//match = GameObject.FindWithTag("GameController").GetComponent<MatchController>();
		}
	
		protected void Update () {
			if (!IsActive()) return;
		
			CheckInputs();
			
			if (ctrl.IsTryingToAttack()) {
				Attack();
			}
			
			if (ctrl.IsTryingToJump()) {
				Jump();
			}
			
			HandleMovement();
			HandleMaxSpeed();
			HandleFootsteps();
			//HandleAnimations();
			
			TickCooldowns();
		}

		private bool IsActive() {
			return true;
		}

		private void CheckInputs() {
			moveX = ctrl.GetMoveX();
			moveY = ctrl.GetMoveY();
			
			aimX = ctrl.GetAimX(transform.position);
			aimY = ctrl.GetAimY(transform.position);
		}

		private void HandleMovement() {
			body.AddForce(new Vector3(
				moveX * speed * Time.fixedDeltaTime * SPEED_FORCE_MULTIPLIER, 
				0, 
				-moveY * speed * Time.fixedDeltaTime * SPEED_FORCE_MULTIPLIER), ForceMode.Force
			);
		}

		private void HandleMaxSpeed() {
		
			body.velocity = new Vector3(
				Mathf.Clamp(body.velocity.x, -MAX_AIRSPEED, MAX_AIRSPEED),
				Mathf.Clamp(body.velocity.y, -MAX_AIRSPEED, MAX_AIRSPEED),
				Mathf.Clamp(body.velocity.z, -MAX_AIRSPEED, MAX_AIRSPEED)
			);
		}

		private void HandleFootsteps() {
			if (sfxFootsteps.Length <= 0) return;
			
			if (body.velocity.x > animationDeadzone 
			    || body.velocity.x < -animationDeadzone 
			    || body.velocity.z > animationDeadzone 
			    || body.velocity.z < -animationDeadzone) {
				footstepsCooldown--;
			}

			bool isTryingToMove = moveX != 0 || moveY != 0;

			if (!isTryingToMove || footstepsCooldown > 0) return;
			
			AudioClip footstepClip = sfxFootsteps[Random.Range(0, sfxFootsteps.Length - 1)];
				
			PlaySFX(footstepClip, 0.3f);
				
			footstepsCooldown = footstepsDelay;
		}

		private void HandleAnimations() {

			animationName = "idle_down";
			
			bool isAttacking = (attackCooldown > (attackDelay - 15));

			if (body.velocity.x > animationDeadzone) animationName = "move_right";
			if (body.velocity.x < -animationDeadzone) animationName = "move_left";
			if (body.velocity.z < -animationDeadzone && body.velocity.z < body.velocity.x) animationName = "move_down";
			if (body.velocity.z > animationDeadzone && body.velocity.z > body.velocity.x) animationName = "move_up";

			animator.Play(animationName);

		}

		private void TickCooldowns() {
			if(attackCooldown > 0) attackCooldown--;
			if(jumpCooldown > 0) jumpCooldown--;

			if (jumpCooldown <= 0) {
				collision.material = doCollideMat;
			}
		}

		public void PlaySFX(AudioClip clip, float volume = 1.0f) {
			AudioSource.PlayClipAtPoint(clip, transform.position, volume);
		}
		
		public void Attack() {

			if (attackCooldown > 0) return;
			
			attackCooldown = attackDelay;
			
			//PlaySFX(sfxAttack);

			GameObject bullet = Instantiate(bulletPrefab, body.position, body.rotation);
			Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

			bullet.GetComponent<Bullet>().Fire(transform.position, aimX, aimY, TeamID.Player);
			
			Debug.DrawRay(body.position, new Vector3(aimX, 0, aimY), Color.blue, 5);

		}

		public void Jump() {
			
			if (jumpCooldown > 0) return;
			jumpCooldown = jumpDelay;

			collision.material = noCollideMat;
			
			Debug.Log("---- Jump! ----");
			
			body.velocity = new Vector3(body.velocity.x, jumpForce, body.velocity.z);
		}
	}
}
