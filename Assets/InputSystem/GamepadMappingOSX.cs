namespace InputSystem {
	public class GamepadMappingOSX : GamepadMapping {

		public GamepadMappingOSX() {
			inputMap[GamepadInput.A] = 16;
			inputMap[GamepadInput.B] = 17;
			inputMap[GamepadInput.X] = 18;
			inputMap[GamepadInput.Y] = 19;
			inputMap[GamepadInput.Back] = 10;
			inputMap[GamepadInput.Start] = 9;
			inputMap[GamepadInput.LB] = 13;
			inputMap[GamepadInput.RB] = 14;
			inputMap[GamepadInput.LStickClick] = 11;
			inputMap[GamepadInput.RStickClick] = 12;
			inputMap[GamepadInput.DPadUp] = 5;
			inputMap[GamepadInput.DPadDown] = 6;
			inputMap[GamepadInput.DPadLeft] = 7;
			inputMap[GamepadInput.DPadRight] = 8;
			inputMap[GamepadInput.System] = 15;
			
			inputMap[GamepadInput.AxisLeftTrigger] = 5;
			inputMap[GamepadInput.AxisRightTrigger] = 6;
			inputMap[GamepadInput.AxisRightStickX] = 3;
			inputMap[GamepadInput.AxisRightStickY] = 4;
			inputMap[GamepadInput.AxisLeftStickX] = 0;
			inputMap[GamepadInput.AxisLeftStickY] = 1;;
		}
	}
}