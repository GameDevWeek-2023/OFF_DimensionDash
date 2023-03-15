using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPoints : MonoBehaviour {
	public readonly  List<GameObject> _playerText = new List<GameObject>();
	public           List<GameObject> _players;
	private          float            _time                 = 0;
	private          float            _secondsPerDimension  = ObjectManager.SecondsPerDimension;
	private readonly float            _secondsCompleteLevel = 50f; //TODO: Specify complete Level time
	private          Slider           _levelSlider;
	private          TMP_Text         _dimensionCountdown;

	private void Start() {
		initializePlayerPointsCanvas(this.gameObject);
		_players              = PlayerManager.Instance.Players;
		_levelSlider          = gameObject.transform.Find("LevelProgressBar").GetComponent<Slider>();
		_levelSlider.maxValue = _secondsCompleteLevel;
		_dimensionCountdown   = gameObject.transform.Find("DimensionCountdown").GetComponent<TMP_Text>();
		_secondsPerDimension  = ObjectManager.SecondsPerDimension;

		for (int i = 0; i < _players.Count; i++) {
			_playerText[i].SetActive(true);
			;
		}

		for (int i = _players.Count; i < _playerText.Count; i++) {
			_playerText[i].SetActive(false);
		}

		setPlayerColor();
	}

	private void setPlayerColor() {
		for (int i = 0; i < _players.Count; i++) {
			Color c = _players[i].GetComponent<PlayerColor>().GetColor().Color;
			_playerText[i].GetComponent<TMP_Text>().color = c;
		}
	}

	void Update() {
		for (int i = 0; i < _players.Count; i++) {
			_playerText[i].GetComponent<TMP_Text>().text = getPointsFromPlayer(_players[i].GetComponent<PlayerPoints>());
		}

		_time += Time.deltaTime;
		float levelProgress = _time % _secondsCompleteLevel;
		_levelSlider.value = levelProgress;
		float dimensionProgress = (float) Math.Round(Math.Abs((_time % _secondsPerDimension) - _secondsPerDimension), 2);
		_dimensionCountdown.text = dimensionProgress.ToString();
	}

	private static string getPointsFromPlayer(PlayerPoints player) {
		return player.points.ToString();
	}

	private void initializePlayerPointsCanvas(GameObject parent) {
		List<GameObject> parents     = new List<GameObject>();
		Transform[]      allChildren = parent.GetComponentsInChildren<Transform>();
		foreach (Transform obj in allChildren) {
			if (obj.transform.parent == parent.transform && obj.name.Contains("Points")) {
				_playerText.Add(obj.gameObject);
			}
		}
	}
}
