using System;
using System.Collections.Generic;
using UnityEngine;

namespace InputSystem {
	public abstract class GamepadMapping {
		
		protected Dictionary<GamepadInput, int> inputMap = new Dictionary<GamepadInput, int>();

		public float GetAxis(int joystick, GamepadInput axis) {
			return Input.GetAxis("Joy" + joystick.ToString() + "_Axis" + inputMap[axis].ToString());
		}
	
		protected KeyCode GetMappedKeyCode(int joystick, GamepadInput button) {
			return (KeyCode) Enum.Parse(typeof(KeyCode), "Joystick" + joystick.ToString() + "Button" + inputMap[button].ToString());
		}
		
		public bool GetButton(int joystick, GamepadInput button) {
			if (!inputMap.ContainsKey(button)) return false;
			return Input.GetKey(GetMappedKeyCode(joystick, button));
		}
	
		public bool GetButtonDown(int joystick, GamepadInput button) {
			if (!inputMap.ContainsKey(button)) return false;
			return Input.GetKeyDown(GetMappedKeyCode(joystick, button));
		}
	
		public bool GetButtonUp(int joystick, GamepadInput button) {
			if (!inputMap.ContainsKey(button)) return false;
			return Input.GetKeyUp(GetMappedKeyCode(joystick, button));
		}
		
	}
}