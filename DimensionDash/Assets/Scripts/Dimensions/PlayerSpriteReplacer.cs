using System;
using UnityEngine;

namespace Dimensions {

	public enum PlayerSpriteType {
		Base,
		Pixel,
		Spider,
		Fish,
		Spaceship,
		Cube,
		FlipperBall,
		Stone,
	}
	
	public class PlayerSpriteReplacer : MonoBehaviour {
		[SerializeField] private PlayerSpriteType                                _initialType = PlayerSpriteType.Base;
		[SerializeField] private GenericDictionary<PlayerSpriteType, GameObject> _typeGameObjects;

		private PlayerSpriteType _currentType;

		private void Start() {
			foreach(var (type, go) in _typeGameObjects)
				go.SetActive(type==_initialType);
			
			_currentType = _initialType;
		}

		public void SetType(PlayerSpriteType newType) {
			if (newType == _currentType)
				return;
			
			if (_typeGameObjects.TryGetValue(newType, out var newGo) && newGo) {
				newGo.SetActive(true);
			}

			if (_typeGameObjects.TryGetValue(_currentType, out var oldGo) && oldGo) {
				oldGo.SetActive(false);
			}

			_currentType = newType;
		}

	}
}
