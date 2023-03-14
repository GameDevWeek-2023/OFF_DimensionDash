using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Menus {
	public class JoinSceneHandler : MonoBehaviour {
		[SerializeField] private List<PlayerJoined> _startPositions;

		private void Start() {
			PlayerManager.Instance.SetStartPositions(_startPositions);
		}
	}
}
