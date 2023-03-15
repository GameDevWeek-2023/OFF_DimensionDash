using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerManager : GlobalSystem<PlayerManager> {
		[SerializeField] private PlayerInputManager _playerInputManager;

		public event Action<GameObject> OnJoin = delegate(GameObject o) { };
		public event Action<GameObject> OnLeft = delegate(GameObject o) { };

		private readonly List<GameObject> _players      = new();
		private          bool             _allowJoining = false;

		public bool AllowJoining {
			get => _allowJoining;
			set {
				_allowJoining = value;
				if (value)
					_playerInputManager.EnableJoining();
				else
					_playerInputManager.DisableJoining();
			}
		}

		public List<GameObject> Players => _players;

		private void Start() {
			AllowJoining = false;
		}

		public void OnPlayerJoined(PlayerInput input) {
			if (AllowJoining) {
				var player = input.transform.GetChild(0).gameObject;
				OnJoin(player);
				if (player)
					_players.Add(player);
				else {
					Destroy(input.gameObject);
				}
			}
		}

		public void OnPlayerLeft(PlayerInput input) {
			if (AllowJoining) {
				var player = input.transform.GetChild(0).gameObject;
				_players.Remove(player);
				OnLeft(player);
			}
		}
	}
}
