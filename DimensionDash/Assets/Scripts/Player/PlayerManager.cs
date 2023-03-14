using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Player {
	public class PlayerManager : GlobalSystem<PlayerManager> {
		private class JoinedPlayers {
			public PlayerController playerController;
			public Color            color;
			public PlayerJoined     joinBox;
		}

		[SerializeField] private List<Color>        _colors;
		[SerializeField] private PlayerInputManager _playerInputManager;
		[SerializeField] private float              _startDelay = 3;
		[SerializeField] private GameStateManager   _gameStateManager;

		private          List<Color>         _availableColor;
		private readonly List<PlayerJoined>  _remainingStartPositions = new();
		private readonly List<JoinedPlayers> _players                 = new();
		private          bool                _allowJoining            = true;
		private          float               _startCountdown          = 3;

		public bool AllowJoining => _allowJoining;

		public void SetStartPositions(List<PlayerJoined> p) {
			_remainingStartPositions.Clear();
			_remainingStartPositions.AddRange(p);
			_remainingStartPositions.Reverse();
		}
		
		private void Start() {
			_availableColor = new List<Color>(_colors);
			_startCountdown = _startDelay;
		}

		protected override void OnEnable() {
			base.OnEnable();
			_gameStateManager.GameStarted += OnGameStarted;
			_gameStateManager.GameEnded   += OnGameEnded;
		}

		private void OnDisable() {
			_gameStateManager.GameStarted -= OnGameStarted;
			_gameStateManager.GameEnded   -= OnGameEnded;
		}

		private void OnGameStarted() { }

		private void OnGameEnded() {
			_allowJoining = true;
			_playerInputManager.EnableJoining();
			_startCountdown = _startDelay;
		}

		private void Update() {
			if (_startCountdown <= 0)
				return; // load already triggered

			if (_players.Count == 0 || _players.Any(p => !p.playerController.Ready)) {
				_startCountdown = _startDelay;
				return;
			}

			_startCountdown -= Time.deltaTime;
			if (_startCountdown > 0)
				return;

			// all ready
			_allowJoining = false;
			_playerInputManager.DisableJoining();

			// TODO: load level select instead
			_gameStateManager.StartGame(0);
		}

		public void OnPlayerJoined(PlayerInput input) {
			if (!_allowJoining || _remainingStartPositions.Count == 0)
				return;

			var colorIndex = Random.Range(0, _availableColor.Count);
			var color      = _availableColor[colorIndex];
			_availableColor.RemoveAt(colorIndex);
			if (input.TryGetComponent(out PlayerColor playerColor)) {
				playerColor.SetColor(color, () => _availableColor.Add(color));
			}

			var positionIndex = _remainingStartPositions.Count - 1;
			var joined        = _remainingStartPositions[positionIndex];
			input.transform.position = joined.transform.position;
			joined.PlayerJoinedMe();
			_remainingStartPositions.RemoveAt(positionIndex);
			_players.Add(new JoinedPlayers() {color = color, joinBox = joined, playerController = input.GetComponent<PlayerController>()});
			Debug.Log("Player joined");
		}

		public void OnPlayerLeft(PlayerInput input) {
			if (_allowJoining) {
				var p = _players.Find(p => p.playerController.gameObject == input.gameObject);
				if (p == null) return;

				_remainingStartPositions.Add(p.joinBox);
				_players.Remove(p);
				Debug.Log("Player left");
			}
		}
	}
}
