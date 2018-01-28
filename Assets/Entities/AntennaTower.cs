using Controllers;
using UnityEngine;

namespace Entities {
	public class AntennaTower : MonoBehaviour {

		public MatchController match;
	
		public bool isAntennaOn = false;
		public bool hasCompletedTransmission = false;
	
		public int antennaWorkComplete = 0;
		public int antennaWorkMax = 500;
		public Light antennaLight;

		public void Start() {
			antennaLight = GetComponentInChildren<Light>();
			match = MatchController.GetInstance();
		}
	
		public void Reset() {
			antennaWorkComplete = 0;
			isAntennaOn = false;
		}
	
		public void Update () {

			if (isAntennaOn) {
				antennaWorkComplete++;
			}

			if (antennaWorkComplete > antennaWorkMax) {
				antennaWorkComplete = antennaWorkMax;
			}
		
			bool isWorkComplete = (antennaWorkComplete >= antennaWorkMax);
		
			antennaLight.enabled = isAntennaOn;
			antennaLight.intensity = 5 * Mathf.Abs((Mathf.Sin(Time.time * 2f)));
			antennaLight.color = isWorkComplete ? Color.green : Color.red;

			if (!isWorkComplete || hasCompletedTransmission) return;
			
			hasCompletedTransmission = true;
			match.OnPlayerCompletedTransmission();
		}

		public void TurnOn(Player player) {
			if (isAntennaOn) return;
			
			Debug.Log("Player " + player.id + " has turned on antenna!");
			isAntennaOn = true;
		}

		private void OnTriggerEnter(Collider player) {
			Debug.Log("ARRIVED at antenna range: " + player.gameObject);
			
			if (!player.gameObject.CompareTag("Player")) return;
			
			player.gameObject.GetComponent<Player>().canUseAntenna = true;
			player.gameObject.GetComponent<Player>().nearbyAntenna = this;
		}

		private void OnTriggerExit(Collider player) {
			Debug.Log("LEFT antenna range: " + player.gameObject);
			
			if (!player.gameObject.CompareTag("Player")) return;
			
			player.gameObject.GetComponent<Player>().canUseAntenna = false;
			player.gameObject.GetComponent<Player>().nearbyAntenna = null;
		
			// TODO: turn antenna off if no players within range
		}
	}
}
