using System.Collections.Generic;
using InputSystem;

namespace Structs {
	public class PlayerSettings {
		public PlayerID id;
		public InputMode inputMode;
		
		public static Dictionary<PlayerID, PlayerSettings> settings = new Dictionary<PlayerID, PlayerSettings>();

		public PlayerSettings(PlayerID id, InputMode inputMode) {
			this.id = id;
			this.inputMode = inputMode;
		}

		public static void Setup(PlayerID id, InputMode mode) {
			settings[id] = new PlayerSettings(id, mode);
		}

		public static InputMode GetInputMode(PlayerID id) {
			if (!settings.ContainsKey(id)) return InputMode.Keyboard; 
			return settings[id].inputMode;
		}
	}
}