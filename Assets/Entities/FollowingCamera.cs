using UnityEngine;

namespace Entities {
	public class FollowingCamera : MonoBehaviour {

		public GameObject player1;
		public GameObject player2;

		public void Update() {
			
			transform.position = new Vector3(player1.transform.position.x, transform.position.y, player1.transform.position.z - 5f);
			
		}
		
	}
}