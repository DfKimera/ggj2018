namespace InputSystem {
	public class GamepadMappingWindows : GamepadMapping {

		public GamepadMappingWindows() {
			inputMap[GamepadInput.A] = 0;
			inputMap[GamepadInput.B] = 1;
			inputMap[GamepadInput.X] = 2;
			inputMap[GamepadInput.Y] = 3;
			inputMap[GamepadInput.Back] = 6;
			inputMap[GamepadInput.Start] = 7;
			inputMap[GamepadInput.LB] = 4;
			inputMap[GamepadInput.RB] = 5;
			inputMap[GamepadInput.LStickClick] = 8;
			inputMap[GamepadInput.RStickClick] = 9;
			inputMap[GamepadInput.System] = 15;
			
			inputMap[GamepadInput.AxisLeftTrigger] = 9;
			inputMap[GamepadInput.AxisRightTrigger] = 10;
			inputMap[GamepadInput.AxisRightStickX] = 4;
			inputMap[GamepadInput.AxisRightStickY] = 5;
			inputMap[GamepadInput.AxisLeftStickX] = 0;
			inputMap[GamepadInput.AxisLeftStickY] = 1;
		}
	}
}