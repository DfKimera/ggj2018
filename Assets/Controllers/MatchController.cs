using Entities;
using Structs;
using UnityEngine;

namespace Controllers {
	public class MatchController : MonoBehaviour {

		public bool HasMatchStarted = false;
		public bool IsGameOver = false;
		public bool IsMenuShowing = true;

		public GameObject mainMenu;
		public GameObject gameOverScreen;

		public void StartGame() {
			Debug.Log("--- START GAME ---");
			
			Player.All.ForEach(ply => ply.ResetToStart());
			
			this.IsGameOver = false;
			this.IsMenuShowing = false;
			this.HasMatchStarted = true;
		}

		public void OnPlayerDeath(Player player) {
			Debug.Log("Registered player death: " + player.id);
			
			var activePlayers = Player.All.FindAll(ply => ply.IsActive());
			var alivePlayers = Player.All.FindAll(ply => ply.IsActive() && ply.IsAlive());
			
			
			Debug.Log("Player count: alive=" + alivePlayers.Count + ", active=" + activePlayers.Count);
			
			// TODO: play dramatic SFX clip

			if (alivePlayers.Count <= 0) {
				Debug.Log("All players are dead! Triggering game over");
				GameOver();
			}
		}

		public void GameOver() {
			Debug.Log("--- GAME OVER ---");
			this.IsGameOver = true;
			this.IsMenuShowing = false;
			this.HasMatchStarted = true;
			
			// TODO: play game over SFX clip
			
			Invoke("BackToMenu", 5.0f);
		}

		public void BackToMenu() {

			if (!this.IsGameOver) return;
			
			Debug.Log("--- BACK TO MENU ---");
			
			this.IsGameOver = false;
			this.IsMenuShowing = true;
			this.HasMatchStarted = false;
		}
		
		public void OpenMenu() {
			Debug.Log("--- OPEN MENU---");
			
			this.IsGameOver = false;
			this.IsMenuShowing = true;
			this.HasMatchStarted = false;
		}

		public void Start() {
			gameObject.tag = "MatchController";
		}

		public void Update() {
			mainMenu.SetActive(this.IsMenuShowing);
			gameOverScreen.SetActive(this.IsGameOver);
		}

		public static MatchController GetInstance() {
			return GameObject.FindWithTag("MatchController").GetComponent<MatchController>();
		}
	}
}
