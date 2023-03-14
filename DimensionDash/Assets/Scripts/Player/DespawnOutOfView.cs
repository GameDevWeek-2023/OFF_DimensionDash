using UnityEngine;

namespace Player {
	public class DespawnOutOfView : MonoBehaviour {
		[SerializeField] private float         _maxOutOfViewTime = 3f;

		private                  float         _outOfViewTime = 0;
		private RectTransform _marker;

		private void Update() {
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
				outOfViewMarkerPosition.x = -camSize.x;
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
						respawner.KillAndRespawn(gameObject);
						return;
					}
				}
				
				// show marker
				if (!_marker) {
					// TODO: get marker from UI (and set its color)
				}
				
				if (_marker) {
					_marker.position = outOfViewMarkerPosition;
					// TODO: test
					_marker.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.zero, new Vector2(outOfViewMarkerPosition.x, outOfViewMarkerPosition.y)));
				}
				
			} else {
				_outOfViewTime = 0;

				if (_marker) {
					// TODO: return marker to UI
				}
			}
		}
	}
}
