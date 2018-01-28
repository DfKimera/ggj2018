using UnityEngine;

namespace Utils {
	public class VectorUtils {

		public static float GetAngleInRad(Vector3 a, Vector3 b) {
			return Mathf.Atan2(a.x - b.x, a.z - b.z);
		}
		
		public static float GetAngleInDeg(Vector3 a, Vector3 b) {
			return Mathf.Atan2(a.x - b.x, a.z - b.z) * Mathf.Rad2Deg;
		}
		
	}
}