using System;
using Canvas;
using UnityEngine;
using UnityEngine.UI;

namespace Player {
	public class DespawnOutOfView : MonoBehaviour {
		[SerializeField] private float        _maxOutOfViewTime = 3f;
		[SerializeField] private int        _dyingDeduction = 5;
		[SerializeField] private PlayerColor  _playerColor;
		[SerializeField] private PlayerPoints _points;

		private float _outOfViewTime = 0;
		private Image _marker;

		private void OnValidate() {
			if (!_playerColor)
				_playerColor = GetComponent<PlayerColor>();
			if (!_points)
				_points = GetComponent<PlayerPoints>();
		}

		private void OnDisable() {
			_outOfViewTime = 0;
			var pool = OutOfViewMarkerPool.InstanceOptional;
			if (pool)
				pool.ReturnMarker(_marker);
			_marker = null;
		}

		private void LateUpdate() {
			var camera = Camera.main;
			if (!camera) return;

			var camRelativePosition     = transform.position - camera.transform.position;
			var camSize                 = new Vector2(camera.aspect, 1f) * camera.orthographicSize;
			var outOfView               = false;
			var outOfViewMarkerPosition = camRelativePosition;

			if (camRelativePosition.x < -camSize.x) {
				outOfViewMarkerPosition.x = -camSize.x;
				outOfView                 = true;
			} else if (camRelativePosition.x > camSize.x) {
				outOfViewMarkerPosition.x = camSize.x;
				outOfView                 = true;
			}

			if (camRelativePosition.y < -camSize.y) {
				outOfViewMarkerPosition.y = -camSize.y;
				outOfView                 = true;
			} else if (camRelativePosition.y > camSize.y) {
				outOfViewMarkerPosition.y = camSize.y;
				outOfView                 = true;
			}

			if (outOfView) {
				_outOfViewTime += Time.deltaTime;
				if (_outOfViewTime > _maxOutOfViewTime) {
					var respawner = RespawnManager.InstanceOptional;
					if (respawner) {
						// TODO: return marker to UI
						_outOfViewTime = 0f;
						if(_points) _points.updatePlayerPoints(-_dyingDeduction);
						respawner.KillAndRespawn(gameObject);
						return;
					}
				}

				// show marker
				if (!_marker) {
					_marker       = OutOfViewMarkerPool.Instance.GetMarker();
					_marker.color = _playerColor.GetColor().Color;
				}

				if (_marker) {
					_marker.rectTransform.position = camera.WorldToScreenPoint(camera.transform.position + outOfViewMarkerPosition * 0.9f);
					var angle = Vector2.SignedAngle(Vector2.right,
					                                new Vector2(outOfViewMarkerPosition.x / camSize.x, outOfViewMarkerPosition.y / camSize.y).normalized);
					angle                               = 45f * Mathf.Round(angle / 45f);
					_marker.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
				}
			} else {
				_outOfViewTime = 0;

				if (_marker) {
					OutOfViewMarkerPool.Instance.ReturnMarker(_marker);
					_marker = null;
				}
			}
		}
	}
}
