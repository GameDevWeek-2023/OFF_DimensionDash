using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using Tilemap;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

namespace Dimensions {
	public class DimensionManager : MonoBehaviour {
		[SerializeField] private PostProcessVolume _postProcessVolume;

		[SerializeField] private DimensionDescription                  _defaultDimension;
		[SerializeField] private float                                 _maxSecondsBetweenSwitch = float.MaxValue;
		[SerializeField] private float                                 _minSecondsBetweenSwitch = 10;
		[SerializeField] private List<DimensionDescription>            _dimensions;
		[SerializeField] private DimensionSelection                    _dimensionSelection;
		[SerializeField] private TileReskinner                         _tileReskinner;
		[SerializeField] private List<GameObject>                      _platformRoots;
		[SerializeField] private GenericDictionary<string, Sprite>     _tilesetPlatformSprites;
		[SerializeField] private GenericDictionary<string, Sprite>     _tilesetCoinSprites;
		[SerializeField] private GenericDictionary<string, GameObject> _tilesetBackgroundRoots;

		[SerializeField] private float _fadeInDuration            = 0.4f;
		[SerializeField] private float _fadeOutDuration           = 0.1f;
		[SerializeField] private float _fadeInChromaticAberration = 1f;
		[SerializeField] private float _fadeInLensDistortion      = 60f;
		[SerializeField] private float _switchTimeScale           = 0.1f;

		private readonly List<(int, DimensionDescription)> _remainingDimensions = new();

		private List<PlayerSpriteReplacer> _playerSprites;
		private List<GameObject>           _players;

		private readonly List<SpriteRenderer> _coins = new();

		private readonly List<List<SpriteRenderer>> _dimensionPlatforms         = new();
		private          int                        _lastDimensionPlatformIndex = -1;

		private DimensionDescription _current;
		private DimensionDescription _transitioningFrom      = null;
		private float?               _transitionOrgTimeScale = null;
		private float                _nextSwitch;

		[SerializeField] private DimensionDescription _forceDimension;

		[Button]
		private void ForceDimension() {
			if (!_forceDimension || _transitionOrgTimeScale.HasValue) return;

			_nextSwitch = Time.time + 999f;
			StartCoroutine(Switch(_current, _forceDimension));
		}


		private void Start() {
			_nextSwitch = Time.time + _minSecondsBetweenSwitch;

			if (_dimensions != null) {
				foreach (var d in _dimensions) {
					if (d)
						_remainingDimensions.Add((d.MaxUsedPerLevel, d));
				}
			}

			if (_dimensionSelection != null) {
				foreach (var d in _dimensionSelection.GetEnabledDimensions()) {
					if (d)
						_remainingDimensions.Add((d.MaxUsedPerLevel, d));
				}
			}

			_playerSprites = new List<PlayerSpriteReplacer>();
			_players       = new List<GameObject>();
			foreach (var ps in PlayerManager.Instance.Players) {
				_players.Add(ps);
				var spriteReplacer = ps.GetComponent<PlayerSpriteReplacer>();
				Assert.IsNotNull(spriteReplacer);
				_playerSprites.Add(spriteReplacer);
			}

			foreach (var coin in GameObject.FindGameObjectsWithTag("Item")) {
				if (coin.TryGetComponent(out SpriteRenderer sprite))
					_coins.Add(sprite);
			}
			
			if (_platformRoots != null) {
				foreach (var root in _platformRoots) {
					if (!root)
						continue;

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
			}

			foreach (var r in _tilesetBackgroundRoots.Values)
				r.SetActive(false);

			if (_defaultDimension) {
				Enable(_defaultDimension);
				_current = _defaultDimension;
			}
		}

		public float GetTimeUntilNextSwitch() {
			return _nextSwitch - Time.time;
		}

		private void OnDestroy() {
			if (_transitionOrgTimeScale.HasValue)
				Time.timeScale = _transitionOrgTimeScale.Value;

			if (_transitioningFrom)
				Disable(_transitioningFrom);
			else if (_current)
				Disable(_current);
		}

		public void Update() {
			if (_nextSwitch > Time.time || _transitionOrgTimeScale.HasValue || _remainingDimensions.Count <= 0 || (_remainingDimensions.Count == 1 && _remainingDimensions[0].Item2 == _current))
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
			if (_remainingDimensions.Count <= 0 || (_remainingDimensions.Count == 1 && _remainingDimensions[0].Item2 == next))
				_nextSwitch = Time.time + 9999; // no more switching

			StartCoroutine(Switch(_current, next));
		}

		private (ChromaticAberration, LensDistortion) GetPostProcessing() {
			if (_postProcessVolume) {
				var chroma = _postProcessVolume.profile.GetSetting<ChromaticAberration>();
				var lens   = _postProcessVolume.profile.GetSetting<LensDistortion>();
				if (lens && chroma)
					return (chroma, lens);
			}

			return (null, null);
		}

		private IEnumerator Switch(DimensionDescription from, DimensionDescription to) {
			_current           = to;
			_transitioningFrom = from;

			var (chroma, lens) = GetPostProcessing();

			_transitionOrgTimeScale = Time.timeScale;

			// fade in effect
			if (chroma && lens) {
				var time = 0f;
				do {
					var t = time / _fadeInDuration;
					chroma.intensity.value =  t * t * _fadeInChromaticAberration;
					lens.intensity.value   =  t * t * _fadeInLensDistortion;
					time                   += Time.unscaledDeltaTime;
					Time.timeScale         =  Mathf.Lerp(_transitionOrgTimeScale.Value, _switchTimeScale, t * t);
					yield return new WaitForEndOfFrame();
				} while (time < _fadeInDuration);

				yield return new WaitForEndOfFrame();
				chroma.intensity.value = _fadeInChromaticAberration;
				lens.intensity.value   = _fadeInLensDistortion;
				Time.timeScale         = 0.1f;
			}

			yield return new WaitForEndOfFrame();

			if (from) Disable(from);
			if (to) Enable(to);
			_transitioningFrom = null;

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

			if (_tilesetCoinSprites.TryGetValue(to.TileSetName ?? "base", out var coinSprite)) {
				foreach (var coin in _coins) {
					coin.sprite = coinSprite;
				}
			}

			// fade out effect
			if (chroma && lens) {
				var time = 0f;
				do {
					var t = time / _fadeOutDuration;
					chroma.intensity.value =  DOVirtual.EasedValue(_fadeInChromaticAberration, 0f, t, Ease.OutElastic, 2f);
					lens.intensity.value   =  DOVirtual.EasedValue(_fadeInLensDistortion,      0f, t, Ease.OutElastic, 2f);
					time                   += Time.unscaledDeltaTime;
					Time.timeScale         =  Mathf.Lerp(_switchTimeScale, _transitionOrgTimeScale.Value, t);
					yield return new WaitForEndOfFrame();
				} while (time < _fadeOutDuration);

				yield return new WaitForEndOfFrame();
				chroma.intensity.value = 0;
				lens.intensity.value   = 0;
				Time.timeScale         = _transitionOrgTimeScale.Value;
			}

			_transitionOrgTimeScale = null;
		}

		static float EaseOutElastic(float t, float b, float c, float d) {
			float ts = (t /= d) * t;
			float tc = ts * t;
			return b + c * (33 * tc * ts + -106 * ts * ts + 126 * tc + -67 * ts + 15 * t);
		}

		private void Enable(DimensionDescription dimension) {
			if (_tileReskinner)
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
			var sound = UI.SoundManager.InstanceOptional;
			if (sound)
				sound.DimensionChangeMusicSwap(dimension.TileSetName ?? "dave");
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
