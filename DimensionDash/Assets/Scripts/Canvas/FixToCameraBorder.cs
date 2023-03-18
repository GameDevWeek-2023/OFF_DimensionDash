using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Canvas {

	enum Border {
		Top, Bottom, Left, Right
	}
	
	public class FixToCameraBorder : MonoBehaviour {

		[SerializeField] private Border _border;
		[SerializeField] private Camera _camera;

		private void Start() {
			if(!_camera)
				_camera = Camera.main;
		}

		private void LateUpdate() {
			var camSize = new Vector2(_camera.aspect, 1f) * _camera.orthographicSize;
			
			switch (_border) {
				case Border.Top:
					transform.position = _camera.transform.position + Vector3.up * camSize.y;
					break;
				case Border.Bottom:
					transform.position = _camera.transform.position + Vector3.down * camSize.y;
					break;
				case Border.Left:
					transform.position = _camera.transform.position + Vector3.left * camSize.x;
					break;
				case Border.Right:
					transform.position = _camera.transform.position + Vector3.right * camSize.x;
					break;
			}
		}
	}
}
