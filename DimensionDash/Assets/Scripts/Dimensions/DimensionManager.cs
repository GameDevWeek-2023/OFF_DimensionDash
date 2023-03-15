using System;
using System.Collections;
using System.Collections.Generic;
using Tilemap;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dimensions {
	public class DimensionManager : MonoBehaviour {
		[SerializeField] private float                      _minSecondsBetweenSwitch = 10;
		[SerializeField] private List<DimensionDescription> _dimensions;
		[SerializeField] private TileReskinner              _tileReskinner;

		private readonly List<(int, DimensionDescription)> _remainingDimensions = new();

		private PlayerSpriteReplacer[] _playerSprites;
		private List<GameObject>       _players;
		
		private DimensionDescription _current;
		private float                _nextSwitch;

		// TODO: merge with Objects.ObjectManager
		
		private void Start() {
			_nextSwitch = Time.time + _minSecondsBetweenSwitch;

			foreach (var d in _dimensions) {
				_remainingDimensions.Add((d.MaxUsedPerLevel, d));
			}

			_playerSprites = FindObjectsOfType<PlayerSpriteReplacer>();
			_players       = new List<GameObject>(_playerSprites.Length);
			foreach (var ps in _playerSprites) {
				_players.Add(ps.gameObject);
			}
		}

		public float GetTimeUntilNextSwitch() {
			return _nextSwitch - Time.time;
		}

		private void OnDestroy() {
			if (_current)
				Disable(_current);
		}

		public void Update() {
			if (_nextSwitch > Time.time || _remainingDimensions.Count <= 1)
				return;

			var nextIndex = Random.Range(0, _remainingDimensions.Count);
			if (_remainingDimensions[nextIndex].Item2 == _current)
				nextIndex = (nextIndex + 1) % _remainingDimensions.Count;

			var (nextCount, next) = _remainingDimensions[nextIndex];
			if (nextCount > 0) {
				if (nextCount == 1)
					_remainingDimensions.RemoveAt(nextIndex);
				else
					_remainingDimensions[nextIndex] = (nextCount - 1, next);
			}

			var duration = Mathf.Clamp(_minSecondsBetweenSwitch, next.MinTime, next.MaxTime);
			_nextSwitch = Time.time + duration;

			StartCoroutine(Switch(_current, next));
			_current = next;
		}

		private IEnumerator Switch(DimensionDescription from, DimensionDescription to) {
			// TODO: animate / camera-effects...

			yield return null;
			
			if (from) Disable(from);
			if (to) Enable(to);
		}

		private void Enable(DimensionDescription dimension) {
			_tileReskinner.SetTileSet(dimension.TileSetName ?? "base");
			
			foreach(var p in _playerSprites)
				p.SetType(dimension.PlayerSprite);
			
			dimension.Apply(_players);
		}
		private void Disable(DimensionDescription dimension) {
			dimension.UnApply(_players);
			
			foreach(var p in _playerSprites)
				if(p)
					p.SetType(PlayerSpriteType.Base);
			
			if(_tileReskinner)
				_tileReskinner.SetTileSet("base");
		}
	}
}
