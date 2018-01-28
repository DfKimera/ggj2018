using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Utils;
public class NpcMovement : MonoBehaviour {


	protected Vector3 OriginPosition;
	protected Quaternion OriginQuaternion;

	private Rigidbody body;

	private Quaternion rotationToQuaternion;

	private Vector3 waltToPosition;

	[HideInInspector]
	public bool inGround = true;

	private bool inRoutation = false;
	private bool inWalk = false;

	public GameObject bulletPrefab;
	// Use this for initialization
	void Start () {
		OriginQuaternion = transform.rotation;
		OriginPosition = transform.position;

		body = GetComponent<Rigidbody>(); 
	}
	
	// Update is called once per frame

	public void rotateToPosition(Vector3 position) {
		
		var targetPoint = new Vector3(position.x, transform.position.y, position.z) - transform.position;
		var targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);

		rotationToQuaternion = targetRotation;
		if (!inRoutation) StartCoroutine("rotate");
	}

	public void rotateToOriginPosition() {
		rotationToQuaternion = OriginQuaternion;
		if (!inRoutation) StartCoroutine("rotate");
	}

	IEnumerator rotate() {
		inRoutation = true;
		while(inRoutation) {
			yield return new WaitForSeconds(.1f);
			
			if (rotationToQuaternion != transform.rotation) {
				transform.rotation = Quaternion.Slerp(transform.rotation, rotationToQuaternion, Time.deltaTime * 15f);
				continue;
			}  
			
			inRoutation = false;
		}
	}
	
	public void goToForward() {
		StartCoroutine("walk");
	}
	IEnumerator walk() {
		inWalk = true;
		
		var targetPosition = OriginPosition;
		targetPosition.z = targetPosition.z-2;

		while(inWalk) {
			yield return new WaitForSeconds(.03f);
			
			if (transform.position == targetPosition) {
				inWalk = false;
				continue;
			}
			
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 2 * Time.deltaTime);
		}
	}

	public void jump(float force = 3f) {
		// if (body.velocity.y != 0.0f) return;
		body.velocity = new Vector3(0, force, 0);
	}

	public void Shoot() {
		Transform target = GetComponent<FieldOfView>().target;

		if(!target) return;

		Debug.Log("SHOOT");
		
		GameObject bullet = Instantiate(bulletPrefab, body.position, body.rotation);
		Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());

		float angleRad = (VectorUtils.GetAngleInDeg(body.position, target.position) + 90) * Mathf.Deg2Rad;

		bullet.GetComponent<Bullet>().Fire(transform.position, Mathf.Cos(angleRad), -Mathf.Sin(angleRad), TeamID.Enemy);
		
		Debug.DrawRay(body.position, new Vector3(Mathf.Cos(angleRad), 0, -Mathf.Sin(angleRad)), Color.blue, 5);
	}
}
