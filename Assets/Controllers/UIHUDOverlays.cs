using System.Collections.Generic;
using System.Linq;
using Entities;
using Structs;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers {
	public class UIHUDOverlays : MonoBehaviour {
	
		private Text[] Texts;
		private Dictionary<PlayerID, string> playerNames = new Dictionary<PlayerID,string>();
		private MatchController match;

		public GameObject antenna;

		public void Start () {

			match = MatchController.GetInstance();
			
			playerNames[PlayerID.Player1] = "Flan";
			playerNames[PlayerID.Player2] = "Gump";
			
			var template = GetComponentInChildren<Text>(true);

			// Setup object pool
			for (int i = 0; i < 4; i++) {
				GameObject.Instantiate(template.gameObject, transform);
			}

			Texts = GetComponentsInChildren<Text>(true);
		}
	
		public void Update () {
			var antennaCtrl = antenna.GetComponent<AntennaTower>();
			var playerList = Player.All.OrderBy(ply => Vector3.Distance(ply.transform.position, Camera.main.transform.position));

			int i = 0;
		
			foreach (var player in playerList) {

				if (!player.IsActive()) continue;
				if (!player.IsAlive()) continue;

				var pos = Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up * 0.7f);

				//if (pos.z < 0) continue;

				Texts[i].text = "<b>" + playerNames[player.id] + "</b>   " + player.health + " HP";

				if (player.canUseAntenna && !antennaCtrl.isAntennaOn) {
					Texts[i].text += "\nPress SHIFT / B to activate antenna";
				}
				
				Texts[i].rectTransform.localPosition = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0);
				Texts[i].enabled = true;

				i++;

				if (i >= Texts.Length) break;

			}

			if (antennaCtrl.isAntennaOn) {
				Text antennaText = Texts[i++];
				
				Vector3 pos = Camera.main.WorldToScreenPoint(antenna.transform.position);
				float percentComplete = Mathf.Round(((float) antennaCtrl.antennaWorkComplete / (float) antennaCtrl.antennaWorkMax) * 100f);
				
				antennaText.rectTransform.localPosition = new Vector3(Mathf.Round(pos.x - 125f), Mathf.Round(pos.y), 0);
				antennaText.enabled = true;

				antennaText.text = (percentComplete < 100f)
					? ("Transmitting... " + percentComplete + "%")
					: "Transmission COMPLETE!";
			}

			for (; i < Texts.Length; i++) {
				Texts[i].enabled = false;
			}
		}
	}
}
