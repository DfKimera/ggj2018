using UnityEngine;

namespace Entities {
	public class FollowingCamera : MonoBehaviour {

		public GameObject player1;
		public GameObject player2;

		public void Update() {
			
			transform.position = new Vector3(
				transform.position.x, 
				player1.transform.position.y + 15f, 
				player1.transform.position.z - 8.5f);
			
		}
		
	}
}