using System;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas {
	[RequireComponent(typeof(RawImage))]
	public class ParallaxBackground : MonoBehaviour {
		private const float _globalScale = 0.05f;
		
		[SerializeField] private float  _speed = 1f;
		[SerializeField] private Camera _camera;
		
		private RawImage _image;

		private void OnValidate() {
			if (!_image) _image = GetComponent<RawImage>();
		}

		private void Awake() {
			if (!_image) _image = GetComponent<RawImage>();
		}

		private void LateUpdate() {
			var rect = _image.uvRect;
			rect.x        = _camera.transform.position.x * _speed * _globalScale;
			_image.uvRect = rect;
		}
	}
}
