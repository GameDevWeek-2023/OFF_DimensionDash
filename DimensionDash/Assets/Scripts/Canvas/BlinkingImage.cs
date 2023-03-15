using System;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas {
	public class BlinkingImage : MonoBehaviour {
		[SerializeField] [Range(0, 10)] private float _frequency = 4f;
		[SerializeField]                private Image _image;

		private void OnValidate() {
			_image = GetComponent<Image>();
		}
		private void Update() {
			var a = Mathf.PingPong(Time.unscaledTime * _frequency, 1f);
			_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, a);
		}
	}
}
