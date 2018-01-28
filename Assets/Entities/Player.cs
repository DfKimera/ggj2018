using System;
using System.Collections.Generic;
using Controllers;
using InputSystem;
using Structs;
using UnityEngine;
using PlayerSettings = Structs.PlayerSettings;
using Random = UnityEngine.Random;

namespace Entities{
	
	public class Player : MonoBehaviour {

		public static List<Player> All = new List<Player>();
		public static Dictionary<PlayerID, Player> ByID = new Dictionary<PlayerID, Player>();
		
		private const float SPEED_FORCE_MULTIPLIER = 500f;
		private const float MAX_SPEED = 3.2f;
		private const float MAX_AIRSPEED = 6f;

		private Rigidbody body;
		private SphereCollider radius;
		private CapsuleCollider collision;
		private Animator animator;
		private InputController ctrl;
		private SpriteRenderer sprite;
	
		public PlayerID id = PlayerID.Player1;

		public PhysicMaterial noCollideMat;
		public PhysicMaterial doCollideMat;
		public GameObject bulletPrefab;

		public string animationSequence = "idle";
		public string animationDirection = "down";
		
		public Vector3 startPosition;
		
		public bool canUseAntenna = false;
		public AntennaTower nearbyAntenna;

		public int health = 100;
		public int maxHealth = 100;

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

		public MatchController match;
	
		protected void Start () {
			body = GetComponent<Rigidbody>();
			collision = GetComponent<CapsuleCollider>();
			animator = GetComponentInChildren<Animator>();
			ctrl = GetComponent<InputController>();
			sprite = GetComponentInChildren<SpriteRenderer>();
			
			health = maxHealth;
			match = MatchController.GetInstance();
		}
	
		protected void Update () {
			if (!IsActive()) {
				sprite.enabled = false;
				collision.enabled = false;
				body.isKinematic = true;
				return;
			}

			sprite.enabled = true;
			collision.enabled = true;
			body.isKinematic = false;
			
			if (!IsAlive()) return;
		
			CheckInputs();
			
			if (ctrl.IsTryingToAttack()) {
				Attack();
			}
			
			if (ctrl.IsTryingToJump()) {
				Jump();
			}

			if (ctrl.IsTryingToInteract() && canUseAntenna && !nearbyAntenna.isAntennaOn) {
				nearbyAntenna.TurnOn(this);
			}
			
			HandleMovement();
			HandleMaxSpeed();
			HandleFootsteps();
			HandleAnimations();
			
			TickCooldowns();
		}

		private void OnEnable() {
			startPosition = transform.position;
			All.Add(this);
			ByID[this.id] = this;
		}

		private void OnDisable() {
			All.Remove(this);
			ByID.Remove(this.id);
		}

		public bool IsActive() {
			if (!match.HasMatchStarted) return false;
			if (match.IsGameOver) return false;
			if (id == PlayerID.Player2 && PlayerSettings.GetInputMode(PlayerID.Player2) == InputMode.Disconnected) return false;
			return true;
		}
		
		public bool IsAlive() {
			return (health > 0);
		}

		public void ResetToStart() {
			transform.position = startPosition;
			body.velocity = Vector3.zero;
			
			health = maxHealth;
			
			canUseAntenna = false;
			nearbyAntenna = null;
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
				Mathf.Clamp(body.velocity.x, -MAX_SPEED, MAX_SPEED),
				Mathf.Clamp(body.velocity.y, -MAX_AIRSPEED, MAX_AIRSPEED),
				Mathf.Clamp(body.velocity.z, -MAX_SPEED, MAX_SPEED)
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

			animationSequence = "idle";
			
			bool isAttacking = (attackCooldown > (attackDelay - 15));

			if (body.velocity.x > animationDeadzone) {
				animationSequence = "walk";
				animationDirection = "right";
			}

			if (body.velocity.x < -animationDeadzone) {
				animationSequence = "walk";
				animationDirection = "left";
			}
			if (body.velocity.z < -animationDeadzone && body.velocity.z < body.velocity.x) {
				animationSequence = "walk";
				animationDirection = "down";
			}
			if (body.velocity.z > animationDeadzone && body.velocity.z > body.velocity.x) {
				animationSequence = "walk";
				animationDirection = "up";
			}

			if (isAttacking) {
				//animationSequence = "attack";
			}

			string animationName = "flan_" + animationSequence + "_" + animationDirection;
			
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
			
			PlaySFX(sfxAttack);

			GameObject bullet = Instantiate(bulletPrefab, body.position, body.rotation);
			Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

			bullet.GetComponent<Bullet>().Fire(transform.position, aimX, aimY, TeamID.Player);
			
			Debug.DrawRay(body.position, new Vector3(aimX, 0, aimY), Color.blue, 5);

		}

		public void ApplyDamage(int damage) {
			health -= damage;
			
			// TODO: hurt sfx

			Debug.Log("Player " + this.id + " sustained damage: " + damage);
			
			if (health <= 0) {
				health = 0;
				
				Debug.Log("Player " + this.id + " has died!");
				match.OnPlayerDeath(this);
				
				// TODO: death SFX
			}
			
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
