using Entities;
using Structs;
using UnityEngine;

namespace Controllers {
	public class MatchController : MonoBehaviour {

		public bool HasMatchStarted = false;
		public bool IsGameOver = false;
		public bool IsMenuShowing = true;
		public bool IsVictoryShowing = false;
		public bool IsTransmissionCompleted = false;

		public GameObject mainMenu;
		public GameObject gameOverScreen;
		public GameObject victoryScreen;

		public GameObject antenna;

		public void StartGame() {
			Debug.Log("--- START GAME ---");
			
			Player.All.ForEach(ply => ply.ResetToStart());
			antenna.GetComponent<AntennaTower>().Reset();
			
			this.IsGameOver = false;
			this.IsMenuShowing = false;
			this.IsVictoryShowing = false;
			this.HasMatchStarted = true;
			this.IsTransmissionCompleted = false;

		}

		public void OnPlayerDeath(Player player) {
			Debug.Log("Registered player death: " + player.id);
			
			var activePlayers = Player.All.FindAll(ply => ply.IsActive());
			var alivePlayers = Player.All.FindAll(ply => ply.IsActive() && ply.IsAlive());
			
			Debug.Log("Player count: alive=" + alivePlayers.Count + ", active=" + activePlayers.Count);
			
			// TODO: play dramatic SFX clip

			if (alivePlayers.Count > 0) return;
			
			Debug.Log("All players are dead! Triggering game over");
			GameOver();
		}

		public void OnPlayerCompletedTransmission() {
			Debug.Log("Player has completed transmission!");
			
			IsTransmissionCompleted = true;
			HasMatchStarted = false; // Freeze game
			
			Invoke("ShowVictoryScreen", 3f);
		}

		public void ShowVictoryScreen() {
			this.IsVictoryShowing = true;
			
			//Invoke("BackToMenu", 5f);
		}

		public void GameOver() {
			Debug.Log("--- GAME OVER ---");
			this.IsGameOver = true;
			this.IsMenuShowing = false;
			this.IsVictoryShowing = false;
			this.HasMatchStarted = true;
			this.IsTransmissionCompleted = false;
			
			// TODO: play game over SFX clip
			
			Invoke("BackToMenu", 5.0f);
		}

		public void BackToMenu() {

			if (!this.IsGameOver && !this.IsTransmissionCompleted) return;
			
			Debug.Log("--- BACK TO MENU ---");
			
			this.IsGameOver = false;
			this.IsMenuShowing = true;
			this.IsVictoryShowing = false;
			this.HasMatchStarted = false;
			this.IsTransmissionCompleted = false;
			this.IsTransmissionCompleted = false;
		}
		
		public void OpenMenu() {
			Debug.Log("--- OPEN MENU---");
			
			this.IsGameOver = false;
			this.IsMenuShowing = true;
			this.IsVictoryShowing = false;
			this.HasMatchStarted = false;
		}

		public void Start() {
			gameObject.tag = "MatchController";
		}

		public void Update() {
			mainMenu.SetActive(this.IsMenuShowing);
			gameOverScreen.SetActive(this.IsGameOver);
			victoryScreen.SetActive(this.IsVictoryShowing);
		}

		public static MatchController GetInstance() {
			return GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
		}
	}
}
