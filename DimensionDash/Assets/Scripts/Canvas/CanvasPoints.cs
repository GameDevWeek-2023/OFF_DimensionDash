using System;
using System.Collections.Generic;
using Dimensions;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPoints : MonoBehaviour {
	[SerializeField] private List<TMP_Text>   _playerText;
	[SerializeField] private TMP_Text         _dimensionCountdown;
	[SerializeField] private Slider           _levelSlider;
	[SerializeField] private Animation        _cameraAnimation;
	[SerializeField] private DimensionManager _dimensionManager;

	private void Start() {
		_levelSlider.maxValue = 1f;

		var playerCount = PlayerManager.Instance.Players.Count;
		for (int i = 0; i < _playerText.Count; i++) {
			_playerText[i].gameObject.SetActive(i < playerCount);
			
			if(i < playerCount)
				_playerText[i].color = PlayerManager.Instance.Players[i].GetComponent<PlayerColor>().GetColor().Color;
		}
	}

	void Update() {
		for (int i = 0; i < PlayerManager.Instance.Players.Count; i++) {
			_playerText[i].text = getPointsFromPlayer(PlayerManager.Instance.Players[i].GetComponent<PlayerPoints>());
		}

		_levelSlider.value = AnimationProgress(_cameraAnimation);

		if (_dimensionManager) {
			var nextSwitch = _dimensionManager.GetTimeUntilNextSwitch();
			if (nextSwitch <= 3f) {
				_dimensionCountdown.gameObject.SetActive(true);
				_dimensionCountdown.text = nextSwitch.ToString("0.0");
			}  else {
				_dimensionCountdown.gameObject.SetActive(false);
			}
		}
	}

	private static float AnimationProgress(Animation a) {
		if (!a)
			return 0f;
		
		foreach (AnimationState state in a) {
			return state.normalizedTime;
		}

		return 1f;
	}

	private static string getPointsFromPlayer(PlayerPoints player) {
		return player.points.ToString();
	}
}
