using System.Linq;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers {
	public class UIPlayerOverlays : MonoBehaviour {
	
	
		private Text[] Texts;

		void Start () {
			var template = GetComponentInChildren<Text>(true);

			// Setup object pool
			for (int i = 0; i < 4; i++) {
				GameObject.Instantiate(template.gameObject, transform);
			}

			Texts = GetComponentsInChildren<Text>(true);
		}
	
		void Update () {
			var playerList = Player.All.OrderBy(ply => Vector3.Distance(ply.transform.position, Camera.main.transform.position));

			int i = 0;
		
			foreach (var player in playerList) {

				if (!player.IsActive()) continue;
				if (!player.IsAlive()) continue;

				var pos = Camera.main.WorldToScreenPoint(player.transform.position + Vector3.up * 0.7f);

				//if (pos.z < 0) continue;

				Texts[i].text = "#" + player.id + ": " + player.health + " HP";
				Texts[i].rectTransform.localPosition = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0);
				Texts[i].enabled = true;

				i++;

				if (i >= Texts.Length) break;

			}

			for (; i < Texts.Length; i++) {
				Texts[i].enabled = false;
			}
		}
	}
}
