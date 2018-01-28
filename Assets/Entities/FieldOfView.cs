using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0,360)]	
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	private Rigidbody body;

	[HideInInspector]
	public Transform targetVisible;

	public int initialAlertTick;

	private int alertTick;

	private bool isAlert;

	private bool isBloodInTheEyes;

	private Quaternion quaternionOrigin;
	protected void Start() {
		body = GetComponent<Rigidbody>();
		isAlert = false;
		quaternionOrigin = transform.rotation;
		StartCoroutine("FindTargetsWithDelay", .3f);
	}
	IEnumerator FindTargetsWithDelay(float delay) {
		while(true) {
			yield return new WaitForSeconds(delay);

			FindVisibleTarget();
			ShootIfIsFocused();
			FocusTargetIfIsVisible();
			AlertTickCount();
		}
	}

	void Update() {
		if (targetVisible != null) {
			RotateToTarget();
			return;
		}
		if (alertTick <= initialAlertTick/4) {
			StartCoroutine("rotateToOrigin");
		}
	}

	void setHardState() {
		isBloodInTheEyes = true;
		alertTick = alertTick*5;
		//StartCoroutine("bloodInTheEyes");
	}

	IEnumerator bloodInTheEyes() {
		var toLeft = true;
		var count = 5;
		while(true) {
			yield return new WaitForSeconds(.1f);
			if (targetVisible == null) continue;
			if (toLeft) {
				transform.eulerAngles = new Vector3(transform.eulerAngles.x+10, transform.eulerAngles.y, transform.eulerAngles.z+10);
			} else {
				transform.eulerAngles = new Vector3(transform.eulerAngles.x-10, transform.eulerAngles.y, transform.eulerAngles.z-10);
			}
			count--;
			if (count < 1) toLeft = !toLeft;
		}
	}

	IEnumerator rotateToOrigin() {
		while(targetVisible == null && transform.rotation != quaternionOrigin) {
			yield return new WaitForSeconds(.1f);
			transform.rotation = Quaternion.Slerp(transform.rotation, quaternionOrigin, Time.deltaTime * 5f);
		}
	}
	void RotateToTarget() {
		var targetPoint = new Vector3(targetVisible.transform.position.x, targetVisible.transform.position.y, targetVisible.transform.position.z) - transform.position;
		var targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
	}

	void ShootIfIsFocused() {
		if (targetVisible == null || !isAlert) return;

		setHardState();
		Debug.Log("SHOOT!");
	}

	void FocusTargetIfIsVisible() {
		if (targetVisible == null) return;
		if (isAlert) return;

		body.velocity = new Vector3(0, 3f, 0);
		alertTick = initialAlertTick;
		isAlert = true;
	}

	void AlertTickCount() {
		if (!isAlert) return;
		if (targetVisible == null) alertTick--;

		if (alertTick < initialAlertTick/4) isBloodInTheEyes = false;
		if (alertTick < 0) isAlert = false;
		
		alertTick--;
	}

	void FindVisibleTarget() {
		targetVisible = null;
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (var i = 0 ; targetsInViewRadius.Length > i ; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			float dstToTarget = Vector3.Distance(transform.position, target.position);

			if (Vector3.Angle(transform.forward, dirToTarget) >= viewAngle / 2) continue;
			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) continue;
			targetVisible = target;
			break;
		}
	}
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
