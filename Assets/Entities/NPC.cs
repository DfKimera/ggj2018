using System;
using InputSystem;
using Structs;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entities{
	
	public class NPC : MonoBehaviour {
		
		private Rigidbody body;

		private SphereCollider radius;
		private CapsuleCollider collision;
		private Animator animator;
		private InputController ctrl;

		public PhysicMaterial noCollideMat;
		public PhysicMaterial doCollideMat;

		public string animationName = "player_idle";
		
		public float animationDeadzone = 0.05f;
		
		public int attackCooldown = 0;
		public int attackDelay = 10;
		public float attackForce = 85.0f;

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
		}
	
		protected void Update () {
			if (!IsActive()) return;
		
			
			//HandleAnimations();
		}

		private bool IsActive() {
			return true;
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

		public void PlaySFX(AudioClip clip, float volume = 1.0f) {
			AudioSource.PlayClipAtPoint(clip, transform.position, volume);
		}
		
		public void Attack() {

			if (attackCooldown > 0) return;
			
			attackCooldown = attackDelay;

			PlaySFX(sfxAttack);
			
			// TODO: fire weapon
			
		}
	}
}
