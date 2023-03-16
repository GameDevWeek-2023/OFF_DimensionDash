using System.Collections;
using System.Collections.Generic;
using Tilemap;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dimensions {
	public class DimensionManager : MonoBehaviour {
		[SerializeField] private DimensionDescription              _defaultDimension;
		[SerializeField] private float                             _maxSecondsBetweenSwitch = float.MaxValue;
		[SerializeField] private float                             _minSecondsBetweenSwitch = 10;
		[SerializeField] private List<DimensionDescription>        _dimensions;
		[SerializeField] private DimensionSelection                _dimensionSelection;
		[SerializeField] private TileReskinner                     _tileReskinner;
		[SerializeField] private List<GameObject>                  _platformRoots;
		[SerializeField] private GenericDictionary<string, Sprite> _tilesetPlatformSprites;
		[SerializeField] private GenericDictionary<string, GameObject> _tilesetBackgroundRoots;

		private readonly List<(int, DimensionDescription)> _remainingDimensions = new();

		private PlayerSpriteReplacer[] _playerSprites;
		private List<GameObject>       _players;

		private readonly List<List<SpriteRenderer>> _dimensionPlatforms         = new();
		private          int                        _lastDimensionPlatformIndex = -1;

		private DimensionDescription _current;
		private float                _nextSwitch;

		private void Start() {
			_nextSwitch = Time.time + _minSecondsBetweenSwitch;

			if (_dimensions != null) {
				foreach (var d in _dimensions) {
					if(d)
						_remainingDimensions.Add((d.MaxUsedPerLevel, d));
				}
			}

			if (_dimensionSelection != null) {
				foreach (var d in _dimensionSelection.GetEnabledDimensions()) {
					if(d)
						_remainingDimensions.Add((d.MaxUsedPerLevel, d));
				}
			}

			_playerSprites = FindObjectsOfType<PlayerSpriteReplacer>();
			_players       = new List<GameObject>(_playerSprites.Length);
			foreach (var ps in _playerSprites) {
				_players.Add(ps.gameObject);
			}

			foreach (var root in _platformRoots) {
				var platforms = new List<SpriteRenderer>();

				for (int i = 0; i < root.transform.childCount; i++) {
					var p = root.transform.GetChild(i);
					if (p.TryGetComponent(out SpriteRenderer sprite)) {
						platforms.Add(sprite);
						sprite.gameObject.SetActive(false);
					}
				}

				_dimensionPlatforms.Add(platforms);
			}

			if (_defaultDimension) {
				Enable(_defaultDimension);
				_current = _defaultDimension;
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
			if (duration > _maxSecondsBetweenSwitch)
				duration = _maxSecondsBetweenSwitch;
			_nextSwitch = Time.time + duration;

			StartCoroutine(Switch(_current, next));
			_current = next;
		}

		private IEnumerator Switch(DimensionDescription from, DimensionDescription to) {
			// TODO: animate / camera-effects...

			yield return null;

			if (from) Disable(from);
			if (to) Enable(to);

			// switch platforms
			if (_dimensionPlatforms.Count > 0) {
				// disable old
				if (_lastDimensionPlatformIndex >= 0) {
					foreach (var p in _dimensionPlatforms[_lastDimensionPlatformIndex]) {
						p.gameObject.SetActive(false);
					}
				}

				// enable new
				_lastDimensionPlatformIndex = (_lastDimensionPlatformIndex + 1) % _dimensionPlatforms.Count;
				if (to && _tilesetPlatformSprites.TryGetValue(to.TileSetName ?? "base", out var platformSprite)) {
					foreach (var p in _dimensionPlatforms[_lastDimensionPlatformIndex]) {
						p.sprite = platformSprite;
					}
				}

				foreach (var p in _dimensionPlatforms[_lastDimensionPlatformIndex]) {
					p.gameObject.SetActive(true);
				}
			}
		}

		private void Enable(DimensionDescription dimension) {
			if(_tileReskinner)
				_tileReskinner.SetTileSet(dimension.TileSetName ?? "base");
			
			// switch backgrounds
			if (dimension && _tilesetBackgroundRoots.TryGetValue(dimension.TileSetName ?? "base", out var root) && root) {
				root.SetActive(true);
			}
			
			foreach (var p in _playerSprites)
				p.SetType(dimension.PlayerSprite);

			var cam = Camera.main;
			if (cam && cam.TryGetComponent(out Animation anim)) {
				foreach (AnimationState s in anim)
					s.speed = dimension.CameraSpeed;
			}
			
			dimension.Apply(_players);
		}

		private void Disable(DimensionDescription dimension) {
			dimension.UnApply(_players);

			var cam = Camera.main;
			if (cam && cam.TryGetComponent(out Animation anim)) {
				foreach (AnimationState s in anim)
					s.speed = 1f;
			}
			
			foreach (var p in _playerSprites)
				if (p)
					p.SetType(PlayerSpriteType.Base);
			
			// switch backgrounds
			if (dimension && _tilesetBackgroundRoots.TryGetValue(dimension.TileSetName ?? "base", out var root) && root) {
				root.SetActive(false);
			}
			
			if (_tileReskinner)
				_tileReskinner.SetTileSet("base");
		}
	}
}
