using UnityEngine;

namespace Player {
	public class PlayerJoined : MonoBehaviour
	{
		[SerializeField]
		private GameObject joinG;

		[SerializeField]
		private GameObject colorSelectG;

		[SerializeField]
		private GameObject startIndicator1;

		[SerializeField]
		private GameObject hitBox;

		private PlayerController player;


		void Awake()
		{
			joinG.SetActive(true);
			colorSelectG.SetActive(false);
			startIndicator1.SetActive(false);
		}


		public void PlayerJoinedMe(PlayerController newPlayer) 
		{
			joinG.SetActive(false);
			colorSelectG.SetActive(true);
			player = newPlayer;
		}

		void Update() {
			if (!player) return;

			var ready = player && player.Ready;
			if (ready) {
				startIndicator1.SetActive(true);
				hitBox.SetActive(false);
				player.gameObject.transform.GetChild(0).gameObject.layer = 6; //player unready layer
			}
			else if (!ready) {
				startIndicator1.SetActive(false);
				hitBox.SetActive(true);
				player.gameObject.transform.GetChild(0).gameObject.layer = 11; // standard player layer
			}
		}

		public void PlayerLeftMe() {
			joinG.SetActive(true);
			colorSelectG.SetActive(false);
		}
	}
}
