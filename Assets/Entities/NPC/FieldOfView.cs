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
	public Transform target;

	public int initialAlertTick;

	private int alertTick;

	private bool isAlert;

	[HideInInspector]
	public bool targetExist;

	protected void Start() {
		body = GetComponent<Rigidbody>();

		StartCoroutine("FindTargetsWithDelay", .3f);
	}
	IEnumerator FindTargetsWithDelay(float delay) {
		while(true) {
			yield return new WaitForSeconds(delay);

			FindVisibleTarget();
		}
	}

	public bool targetLocked() {
		if (!targetExist) return false;

		var targetPoint = new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position;
		var targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);

		return targetRotation == transform.rotation;
	}

	void FindVisibleTarget() {
		target = null;
		targetExist = false;
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (var i = 0 ; targetsInViewRadius.Length > i ; i++) {
			Transform _target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (_target.position - transform.position).normalized;
			float dstToTarget = Vector3.Distance(transform.position, _target.position);

			if (Vector3.Angle(transform.forward, dirToTarget) >= viewAngle / 2) continue;
			if (Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) continue;

			target = _target;
			targetExist = true;
			
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