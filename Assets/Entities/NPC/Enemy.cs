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
	
	// Update is called once per frame
	void Update () {
		if (View.targetExist) Movement.rotateToPosition(View.target.position);
	}

	IEnumerator RefreshEnemyState(float cycleTime) {
		while(true) {
			yield return new WaitForSeconds(cycleTime);
			if (State == "enemyAvisted" && !View.targetExist) SetState("enemyLost");
			else if ((State == "inAlert" || State == "enemyLost") && View.targetExist) SetState("enemyAvisted");
			else if (State == "goodVibe" && View.targetExist) SetState("surprise");
		}
	}

	IEnumerator enemyAvistedBehavior() {
		Movement.goToForward();
		while(State == "enemyAvisted") {
			yield return new WaitForSeconds(.5f);
			Movement.Shoot();
		}
	}

	IEnumerator inAlertBehavior() {
		tickStateCount = StateTimeBase*10;
		while(State == "inAlert") {
			yield return new WaitForSeconds(.1f);

			if (tickStateCount == 0 && State == "inAlert") {
				SetState("goodVibe");
			} 

			tickStateCount--;
		}
	}

	IEnumerator enemyLostBehavior() {
		tickStateCount = StateTimeBase*20;
		while(State == "enemyLost") {
			yield return new WaitForSeconds(.1f);

			if (tickStateCount == 0 && State == "enemyLost") {
				SetState("inAlert");
			}

			tickStateCount--;
		}
	}

	IEnumerator goodVibeBehavior() {
		Debug.Log("Rotate to origin.");
		Movement.rotateToOriginPosition();
		while(State == "goodVibe") {
			yield return new WaitForSeconds(.1f);

			//none;
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
		Debug.Log("State changed to: "+ state);
		State = state;
		StartCoroutine(State+"Behavior");
	}
}
