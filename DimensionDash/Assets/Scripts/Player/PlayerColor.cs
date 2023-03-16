using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Player {
	public class PlayerColor : MonoBehaviour {
		[SerializeField] private List<SpriteRenderer> _sprites;

		private ColorInfo _currentColor;
		private Action    _onReturn;

		public void SetColor(ColorInfo color, Action onReturn) {
			if (_onReturn != null) _onReturn();

			_currentColor = color;
			_onReturn     = onReturn;

			foreach (var s in _sprites)
				s.color = color.Color;
		}

		public ColorInfo GetColor() {
			return _currentColor;
		}

		private void OnDestroy() {
			if (_onReturn != null) _onReturn();
		}

		[Button]
		private void AddAllSprites() {
			_sprites.Clear();
			_sprites.AddRange(GetComponentsInChildren<SpriteRenderer>(includeInactive: true));
		}
	}
}
