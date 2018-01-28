using System;
using System.Collections.Generic;
using Entities;
using Structs;
using UnityEngine;
using Utils;

namespace InputSystem {
	public class InputController : MonoBehaviour {
		
		public PlayerID playerID = PlayerID.Player1;
		public InputMode inputMode = InputMode.Keyboard;
		public Vector3 mousePosition;
		
		private GamepadMapping gamepad;
		
		private Dictionary<string, GamepadInput> axisMap = new Dictionary<string, GamepadInput>();
		private Dictionary<string, GamepadInput> gamepadMap = new Dictionary<string, GamepadInput>();
		private Dictionary<string, KeyCode> keyboardMap = new Dictionary<string, KeyCode>();

		public InputController() {

			axisMap["MoveX"] = GamepadInput.AxisLeftStickX;
			axisMap["MoveY"] = GamepadInput.AxisLeftStickY;
			axisMap["AimX"] = GamepadInput.AxisRightStickX;
			axisMap["AimY"] = GamepadInput.AxisRightStickY;

			gamepadMap["Jump"] = GamepadInput.A;
			gamepadMap["Shoot"] = GamepadInput.RB;
			
			keyboardMap["Jump"] = KeyCode.Space;
			keyboardMap["Shoot"] = KeyCode.LeftShift;

			bool isWindows = (Application.platform == RuntimePlatform.WindowsPlayer ||
			                  Application.platform == RuntimePlatform.WindowsEditor); 
			
			gamepad = isWindows
				? (GamepadMapping) new GamepadMappingWindows()
				: (GamepadMapping) new GamepadMappingOSX();
		}

		public void FixedUpdate() {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (!Physics.Raycast(ray, out hit)) return;

			mousePosition = hit.point;
			
			Debug.DrawRay(mousePosition, Vector3.up, Color.red);
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
			return Input.GetAxis(string.Format("KB_Axis{0}", axis));
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

		public float GetRelativeMouseAngle(Vector3 relativePos) {
			return (VectorUtils.GetAngleInDeg(relativePos, mousePosition) + 90);
		}

		public float GetAimX(Vector3 playerPos) {
			return (GetInputMode() == InputMode.Gamepad)
				? GetAxis("AimX")
			 	: Mathf.Cos(GetRelativeMouseAngle(playerPos) * Mathf.Deg2Rad);
		}

		public float GetAimY(Vector3 playerPos) {
			return (GetInputMode() == InputMode.Gamepad)
				? -GetAxis("AimY")
				: -Mathf.Sin(GetRelativeMouseAngle(playerPos) * Mathf.Deg2Rad);
		}

		public bool IsTryingToJump() {
			return GetInputMode()  == InputMode.Gamepad ? WasGamepadButtonPressed(gamepadMap["Jump"]) : Input.GetKey(keyboardMap["Jump"]);
		}

		public bool IsTryingToAttack() {
			return GetInputMode()  == InputMode.Gamepad ? WasGamepadButtonPressed(gamepadMap["Shoot"]) : Input.GetKey(keyboardMap["Shoot"]);
		}
	}
}