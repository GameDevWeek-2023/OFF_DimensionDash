using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Player {
	public class PlayerManager : MonoBehaviour {

		[SerializeField] private List<Color> _colors;

		private List<Color> _availableColor;

		// TODO: implement actual joining logic in separate screen and transfer the resulting players into the level
		
		public void OnPlayerJoined(PlayerInput input) {
			if (input.TryGetComponent(out PlayerColor playerColor)) {
				var colorIndex = Random.Range(0, _colors.Count);
				var color     = _colors[colorIndex];
				_colors.RemoveAt(colorIndex);
				playerColor.SetColor(color, () => _colors.Add(color));
			}

			input.transform.position = transform.position;
		}
		
	}
}
