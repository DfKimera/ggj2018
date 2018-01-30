using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Utils;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour {

	public GameObject bulletPrefab;

	protected Vector3 OriginalPosition;
	protected Quaternion OriginalQuaternion;

	protected Rigidbody Body;
	protected NavMeshAgent MovementAgent;

	private Quaternion RotationToQuaternion;

	void Start () {
		OriginalQuaternion = transform.rotation;
		OriginalPosition = transform.position;
		MovementAgent = GetComponent<NavMeshAgent>();
		Body = GetComponent<Rigidbody>(); 
	}
	
	public Vector3 getOriginalPosition() { return OriginalPosition; }
	public Quaternion getOriginalQuaternion() { return OriginalQuaternion; }

	public void lookAt(float AngleY) {
		lookAt(new Vector3(0, AngleY, 0));
	}
	public void lookAt(Vector3 Angle) {
		lookAt(Quaternion.Euler(Angle));
	}

	public void lookAt(Quaternion rotation) {
		RotationToQuaternion = rotation;
		if (MovementAgent.updateRotation) StartCoroutine("rotate");
	}
	public void goTo(Vector3 position, bool lockCamera = true) {
		MovementAgent.updateRotation = true;
		MovementAgent.destination = position;
	}
	public void lookAtPosition(Vector3 position) {
		var targetPoint = new Vector3(position.x, transform.position.y, position.z) - transform.position;
		var targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);
		lookAt(targetRotation);
	}

	IEnumerator rotate() {
		MovementAgent.updateRotation = false;

		while(!MovementAgent.updateRotation) {
			yield return new WaitForSeconds(.01f);

			if (RotationToQuaternion != transform.rotation) {
				transform.rotation = Quaternion.Slerp(transform.rotation, RotationToQuaternion, .02f);
				continue;
			}  
			
			MovementAgent.updateRotation = true;
		}
	}
	public void ForceUpdateRotation() {
		MovementAgent.updateRotation = true;
	}
	public void jump(float force = 5f) {
		Body.velocity = new Vector3(0, force, 0);
	}

	public void Shoot() {
		Transform target = GetComponent<FieldOfView>().getTarget();

		if(!target) return;

		Debug.Log("SHOOT");
		
		GameObject bullet = Instantiate(bulletPrefab, Body.position, Body.rotation);
		Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

		float angleRad = (VectorUtils.GetAngleInDeg(Body.position, target.position) + 90) * Mathf.Deg2Rad;

		bullet.GetComponent<Bullet>().Fire(transform.position, Mathf.Cos(angleRad), -Mathf.Sin(angleRad), TeamID.Enemy);
		
		Debug.DrawRay(Body.position, new Vector3(Mathf.Cos(angleRad), 0, -Mathf.Sin(angleRad)), Color.blue, 5);
	}

	public bool isStay() {
		return MovementAgent.destination.x == Body.position.x && MovementAgent.destination.z == Body.position.z;
	}

	public bool isRotationStay() {
		return RotationToQuaternion == transform.rotation || MovementAgent.updateRotation;
	}
}
