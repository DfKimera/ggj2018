﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0,360)]	
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	private bool isAlert = false;
	private Rigidbody body;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	private int alertTick;
	protected void Start() {
		body = GetComponent<Rigidbody>();

		Debug.Log("START!");
		StartCoroutine("FindTargetsWithDelay", .3f);
		FindVisibleTargets();
	}

	protected void Update() {
		if (alertTick < 1) {
			isAlert = false;
		}
	}
	private void setAlert(bool status = true) {

		Debug.Log("ALERT SETTED.");
		if (isAlert && status) {
			Shoot();
			return;
		}

		alertTick = 4;
		isAlert = true;
		body.velocity = new Vector3(0, 2f, 0);
	}
	IEnumerable FindTargetsWithDelay(float delay) {
		while(true) {
			yield return new WaitForSeconds(delay);
			Debug.Log("TARGETS SCAN.");
			FindVisibleTargets();
		}
	}

	void Shoot() {
		Debug.Log("SHOOT!");
	}
	void FindVisibleTargets() {
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (int i =0; i < targetsInViewRadius.Length ; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) {
				float dstToTarget = Vector3.Distance(transform.position, target.position);

				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) {
					visibleTargets.Add(target);
				}
			}
		}

		alertTick--;
		if (visibleTargets.Count != 0) {
			setAlert();
		}
	}
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
