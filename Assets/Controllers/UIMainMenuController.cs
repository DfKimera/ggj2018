using InputSystem;
using Structs;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers {
	public class UIMainMenuController : MonoBehaviour {

		public InputMode P1Mode = InputMode.Keyboard;
		public InputMode P2Mode = InputMode.Disconnected;
		public MatchController match;

		public Text P1ModeLabel;
		public Text P2ModeLabel;

		public void Start() {
			match = MatchController.GetInstance();
			
			PlayerSettings.Setup(PlayerID.Player1, P1Mode);
			PlayerSettings.Setup(PlayerID.Player2, P2Mode);
		}
		
		public void SwapP1Input() {
			switch (P1Mode) {
				case InputMode.Keyboard: P1Mode = InputMode.Gamepad; break;
				case InputMode.Gamepad: P1Mode = InputMode.Keyboard; break;
			}
			
			PlayerSettings.Setup(PlayerID.Player1, P1Mode);
		}
		
		public void SwapP2Input() {
			switch (P2Mode) {
				case InputMode.Keyboard: P2Mode = InputMode.Gamepad; break;
				case InputMode.Gamepad: P2Mode = InputMode.Disconnected; break;
				case InputMode.Disconnected: P2Mode = InputMode.Keyboard; break;
			}
			
			PlayerSettings.Setup(PlayerID.Player2, P2Mode);
		}

		public void StartGame() {
			match.StartGame();
		}

		private string RenderText(PlayerID id, InputMode mode) {
			string playerLabel = "Player " + (int)id;
			
			switch (mode) {
				case InputMode.Keyboard: return playerLabel + ": Keyboard";
				case InputMode.Gamepad: return playerLabel + ": Gamepad";
				case InputMode.Disconnected: return playerLabel + ": <not playing>";
				default: return "<unknown>";
			}
		}

		public void Update() {
			P1ModeLabel.text = RenderText(PlayerID.Player1, P1Mode);
			P2ModeLabel.text = RenderText(PlayerID.Player2, P2Mode);
		}
	}
}