using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Menus {
	public class JoinSceneHandler : MonoBehaviour {
		[SerializeField] private List<PlayerJoined> _startPositions;

		private class JoinedPlayers {
			public GameObject       gameObject;
			public PlayerController playerController;
			public ColorInfo        color;
			public PlayerJoined     joinBox;
		}

		[SerializeField] private List<ColorInfo> _colors;
		[SerializeField] private float           _startDelay = 3;

		private          List<ColorInfo>     _availableColor;
		private          List<PlayerJoined>  _remainingStartPositions;
		private readonly List<JoinedPlayers> _players        = new();
		private          float               _startCountdown = 3;

		private void Start() {
			_remainingStartPositions = new List<PlayerJoined>(_startPositions);
			_remainingStartPositions.Reverse();

			_availableColor = new List<ColorInfo>(_colors);
			_startCountdown = _startDelay;

			// add preexisting players
			foreach (var p in PlayerManager.Instance.Players) {
				ColorInfo color = null;

				if (p.TryGetComponent(out PlayerColor playerColor)) {
					color = playerColor.GetColor();
					_colors.Remove(color);
				}
				
				if (p.TryGetComponent(out PlayerPoints points)) {
					points.Reset();
				}

				var controller = p.transform.parent.GetComponent<PlayerController>();
				if(!controller)
					Debug.LogError("Missing PlayerController on joined player");
				controller.Ready = false;
				
				var joined = AssignPlayerPosition(p, controller);
				
				_players.Add(new JoinedPlayers() {gameObject=p, color = color, joinBox = joined, playerController = controller});
			}

			PlayerManager.Instance.AllowJoining = true;

			PlayerManager.Instance.OnJoin += OnJoin;
			PlayerManager.Instance.OnLeft += OnLeft;
		}

		private void OnDisable() {
			var playerManager = PlayerManager.InstanceOptional;
			if (playerManager == null)
				return;
			
			playerManager.AllowJoining = false;

			playerManager.OnJoin -= OnJoin;
			playerManager.OnLeft -= OnLeft;
		}

		private void OnJoin(GameObject player) {
			if (!player)
				return;
			if (_remainingStartPositions.Count == 0) {
				Destroy(player);
				return;
			}

			var colorIndex = Random.Range(0, _availableColor.Count);
			var color      = _availableColor[colorIndex];
			_availableColor.RemoveAt(colorIndex);
			if (player.TryGetComponent(out PlayerColor playerColor)) {
				playerColor.SetColor(color, () => _availableColor.Add(color));
			}

			var controller = player.transform.parent.GetComponent<PlayerController>();
			if(!controller)
				Debug.LogError("Missing PlayerController on joined player");
			
			var joined = AssignPlayerPosition(player, controller);
			_players.Add(new JoinedPlayers() {gameObject = player, color = color, joinBox = joined, playerController = controller});
			Debug.Log("Player joined");
		}

		private PlayerJoined AssignPlayerPosition(GameObject player, PlayerController playerController) {
			var positionIndex = _remainingStartPositions.Count - 1;
			var joined        = _remainingStartPositions[positionIndex];
			player.transform.position = joined.transform.position;
			joined.PlayerJoinedMe(playerController);
			_remainingStartPositions.RemoveAt(positionIndex);
			return joined;
		}

		private void OnLeft(GameObject player) {
			var p = _players.Find(p => p.gameObject == player);
			if (p == null) return;
			p.joinBox.PlayerLeftMe();
			_remainingStartPositions.Add(p.joinBox);
			_players.Remove(p);
			Debug.Log("Player left");
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
			PlayerManager.Instance.AllowJoining = false;

			// TODO: load level select instead
			GameStateManager.Instance.StartGame(0);
		}
	}
}
