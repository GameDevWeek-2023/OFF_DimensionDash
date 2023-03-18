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
			var ready = player && player.Ready;
			if (ready && !startIndicator1.activeSelf) {
				startIndicator1.SetActive(true);
				hitBox.SetActive(false);
			}
			else if (!ready && startIndicator1.activeSelf) {
				startIndicator1.SetActive(false);
				hitBox.SetActive(true);
			}
		}

		public void PlayerLeftMe() {
			joinG.SetActive(true);
			colorSelectG.SetActive(false);
		}
	}
}
