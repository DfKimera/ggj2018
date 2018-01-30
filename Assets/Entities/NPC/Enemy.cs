using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour {

	[HideInInspector]
	public FieldOfView View;

	[HideInInspector]
	public NpcMovement Movement;
	protected string State;
	private bool isBloodInTheEyes;

	public int StateTimeBase;
	private int tickStateCount;
	// Use this for initialization
	void Start () {

		View = GetComponent<FieldOfView>();
		Movement = GetComponent<NpcMovement>();
		
		SetState("goodVibe");
		StartCoroutine("RefreshEnemyState", .5f);
	}

	IEnumerator RefreshEnemyState(float cycleTime) {
		while(true) {
			yield return new WaitForSeconds(cycleTime);
			if (State == "enemyAvisted" && !View.TargetExist()) SetState("enemyLost");
			else if ((State == "inAlert" || State == "enemyLost") && View.TargetExist()) SetState("enemyAvisted");
			else if (State == "goodVibe" && View.TargetExist()) SetState("surprise");
		}
	}

	IEnumerator enemyAvistedBehavior() {
		while(State == "enemyAvisted") {
			yield return new WaitForSeconds(.5f);
			Movement.Shoot();
		}
	}

	IEnumerator inAlertBehavior() {
		tickStateCount = StateTimeBase*30;
		Debug.Log("GOTO IN ALERT");

		Movement.goTo(Movement.getOriginalPosition());
		while(State == "inAlert" && !Movement.isStay()) yield return new WaitForSeconds(.3f);

		Debug.Log("LOOKAT IN ALERT");
		Movement.lookAt(Movement.getOriginalQuaternion());
		while(State == "inAlert" && !Movement.isRotationStay()) yield return new WaitForSeconds(.1f);

		Debug.Log("WHILE IN ALERT");
		float lookAroundAngle = 60f;
		while(State == "inAlert") {
			yield return new WaitForSeconds(.1f);
			float rotateYTo = Movement.getOriginalQuaternion().eulerAngles.y+lookAroundAngle;
			Movement.lookAt(rotateYTo);

			if (Movement.isRotationStay()) lookAroundAngle *= -1;

			if (tickStateCount == 0 && State == "inAlert") {
				SetState("goodVibe");
			} 

			tickStateCount--;
		}
	}	

	IEnumerator enemyLostBehavior() {
		
		Movement.goTo(View.getTargetLastSeen());
		while(State == "enemyLost" && !Movement.isStay()) yield return new WaitForSeconds(.3f);

		tickStateCount = StateTimeBase*20;

		float lookAroundAngle = 60f;
		while(State == "enemyLost") {
			yield return new WaitForSeconds(.1f);

			float rotateYTo = Movement.getOriginalQuaternion().eulerAngles.y+lookAroundAngle;
			Movement.lookAt(rotateYTo);

			if (Movement.isRotationStay()) lookAroundAngle = lookAroundAngle == 0 ? 180 : 0;

			if (tickStateCount == 0 && State == "enemyLost") {
				SetState("inAlert");
			}

			tickStateCount--;
		}
	}

	IEnumerator goodVibeBehavior() {
		Movement.lookAt(Movement.getOriginalQuaternion());
		while(State == "goodVibe") {
			yield return new WaitForSeconds(1f);;
		}
	}

	IEnumerator surpriseBehavior() {
		Movement.jump();
		while(true) {
			yield return new WaitForSeconds(.49f);
			
			SetState("inAlert");
			
			break;
		}
	}

	void SetState(string state) {
		if (state == State) return;

		State = state;
		StartCoroutine(State+"Behavior");
	}
}
