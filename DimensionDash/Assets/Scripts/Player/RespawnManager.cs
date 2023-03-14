using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Player {
	public class RespawnManager : GlobalSystem<RespawnManager> {
		[SerializeField] private float     _respawnDelay = 3;
		[SerializeField] private Vector3[] _respawnPositions;
		[SerializeField] private Transform _respawnPositionSource;
		[SerializeField] private Camera    _camera;
		private                  float     _nextExecution = float.MaxValue;

		private readonly List<(float, GameObject)> _respawnAt = new();


		public void KillAndRespawn(GameObject player) {
			var t = Time.time + _respawnDelay;
			player.SetActive(false);
			_respawnAt.Add((t, player));
			_nextExecution = Mathf.Min(t, _nextExecution);
		}

		private void Update() {
			if (_nextExecution > Time.time)
				return;
			
			_respawnAt.RemoveAll(x => {
				var (t, go) = x;
				if (t > Time.time) return false;

				var p = GetRespawnPosition();
				if (p == null) {
					_nextExecution = Time.time + 1f;
					Debug.Log("No valid respawn position");
					return false;
				}

				go.transform.position = p.Value;
				go.SetActive(true);
				return true;
			});
			
			if(_respawnAt.Count==0)
				_nextExecution = float.MaxValue;
		}

		[Button]
		private void AutoFillWithChildren() {
			var t = _respawnPositionSource ? _respawnPositionSource : transform;
			
			_respawnPositions = new Vector3[t.childCount];
			for (int i = 0; i < t.childCount; i++)
				_respawnPositions[i] = t.GetChild(i).position;
		}

		public Vector3? GetRespawnPosition() {
			if (!_camera) return null;

			var camSize = new Vector2(_camera.aspect, 1f) * _camera.orthographicSize * 0.75f;

			foreach (var p in _respawnPositions) {
				var camRelativePosition = p - _camera.transform.position;
				if (Mathf.Abs(camRelativePosition.x) < camSize.x && Mathf.Abs(camRelativePosition.y) < camSize.y)
					return p;
			}

			return null;
		}
	}
}
