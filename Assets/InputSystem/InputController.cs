using System.Collections.Generic;
using Entities;
using Structs;
using UnityEngine;

namespace InputSystem {
	public class InputController : MonoBehaviour {
		
		public PlayerID playerID = PlayerID.Player1;
		public InputMode inputMode = InputMode.Keyboard;
		
		private GamepadMapping gamepad;
		
		private Dictionary<string, GamepadInput> axisMap = new Dictionary<string, GamepadInput>();
		private Dictionary<string, GamepadInput> gamepadMap = new Dictionary<string, GamepadInput>();
		private Dictionary<string, KeyCode> keyboardMap = new Dictionary<string, KeyCode>();

		public InputController() {

			axisMap["MoveX"] = GamepadInput.AxisLeftStickX;
			axisMap["MoveY"] = GamepadInput.AxisLeftStickY;
			axisMap["AimX"] = GamepadInput.AxisRightStickX;
			axisMap["AimY"] = GamepadInput.AxisRightStickY;
			axisMap["Shoot"] = GamepadInput.AxisRightTrigger;

			gamepadMap["Jump"] = GamepadInput.A;
			
			keyboardMap["Jump"] = KeyCode.Space;
			keyboardMap["Shoot"] = KeyCode.LeftShift;

			bool isWindows = (Application.platform == RuntimePlatform.WindowsPlayer ||
			                  Application.platform == RuntimePlatform.WindowsEditor); 
			
			gamepad = isWindows
				? (GamepadMapping) new GamepadMappingWindows()
				: (GamepadMapping) new GamepadMappingOSX();
		}

		private InputMode GetInputMode() {
			return inputMode; // TODO: load from settings
			//return PlayerSettings.GetInputMode(playerID);
		}

		private int GetPlayerGamepad() {
			switch (playerID) {
				default: case PlayerID.Player1: return 1;
				case PlayerID.Player2: return 2;
			}
		}

		private float GetGamepadAxis(string axis) {
			return gamepad.GetAxis(GetPlayerGamepad(), axisMap[axis]);
		}

		private float GetKeyboardAxis(string axis) {
			return Input.GetAxis("KB_Axis" + axis);
		}

		private bool WasGamepadButtonPressed(GamepadInput button) {
			return gamepad.GetButton(GetPlayerGamepad(), button);
		}

		private bool IsGamepadButtonDown(GamepadInput button) {
			return gamepad.GetButtonDown(GetPlayerGamepad(), button);
		}

		private bool IsGamepadButtonUp(GamepadInput button) {
			return gamepad.GetButtonUp(GetPlayerGamepad(), button);
		}

		public float GetAxis(string axis) {
			return GetInputMode() == InputMode.Gamepad ? GetGamepadAxis(axis) : GetKeyboardAxis(axis);
		}

		public float GetMoveX() {
			return GetAxis("MoveX");
		}

		public float GetMoveY() {
			return GetAxis("MoveY");
		}

		public float GetAimX(Vector3 pos) {
			// TODO: check if KB, get mouse pos aim instead
			return GetAxis("AimX");
		}

		public float GetAimY(Vector3 pos) {
			// TODO: check if KB, get mouse pos aim instead
			return GetAxis("AimY");
		}

		public bool IsTryingToJump() {
			return GetInputMode()  == InputMode.Gamepad ? WasGamepadButtonPressed(gamepadMap["Jump"]) : Input.GetKey(keyboardMap["Jump"]);
		}

		public bool IsTryingToAttack() {
			return GetInputMode()  == InputMode.Gamepad ? (GetGamepadAxis("Shoot") > 0) : Input.GetKey(keyboardMap["Shoot"]);
		}
	}
}