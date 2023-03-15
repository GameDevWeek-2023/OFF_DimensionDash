using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Player;
using TMPro;
using UnityEngine;

public class EndPoints : MonoBehaviour {
	[SerializeField] private float _delay;
	[SerializeField] [Scene] private string _nextScene;

	[SerializeField]
	private GameObject player1,
	                   player2,
	                   player3,
	                   player4,
	                   player5,
	                   player6,
	                   player7,
	                   player8,
	                   player1_Points,
	                   player2_Points,
	                   player3_Points,
	                   player4_Points,
	                   player5_Points,
	                   player6_Points,
	                   player7_Points,
	                   player8_Points,
	                   winnerText;
	
	[SerializeField]
	private TMP_Text _timerText;

	private float _delayLeft;
	
	void Start() {
		_delayLeft = _delay;
		
		GameObject[] playerPoints      = new[] { player1, player2, player3, player4, player5, player6, player7, player8 };
		GameObject[] playernames = new[] {player1_Points, player2_Points, player3_Points, player4_Points, player5_Points, player6_Points, player7_Points, player8_Points};

		var winnerNumber = changePlayerStats(playernames, playerPoints);
		setPlayerColors(playernames, playerPoints);
		setWinnerText(winnerNumber.Item1, winnerNumber.Item2);
	}

	private void Update() {
		if (_delayLeft <= 0)
			return;

		_delayLeft -= Time.unscaledDeltaTime;
		if (_timerText)
			_timerText.text = Mathf.RoundToInt(_delayLeft).ToString();
		
		if (_delayLeft <= 0) {
			GameStateManager.Instance.ChangeScene(_nextScene);
		}
	}

	private void setWinnerText(ColorInfo player, float score) {
		var txt = winnerText.GetComponent<TMP_Text>();
		txt.text  = player.Name+" Wins";
		txt.color = player.Color;
	}
	
	private (ColorInfo,float) changePlayerStats(GameObject[] playernames, GameObject[] playerPoints) {
		var players = PlayerManager.Instance.Players;
		
		var winner = ((ColorInfo)null, float.MinValue);
		for (int player = 0; player < players.Count; player++) {
			var color = players[player].GetComponent<PlayerColor>().GetColor();
			
			playerPoints[player].SetActive(true);
			playernames[player].SetActive(true);
			int points = getPointsFromPlayer(players[player].GetComponent<PlayerPoints>());
			var name = playernames[player].GetComponent<TMP_Text>();
			name.text                                          = color.Name;
			name.color                                         = color.Color;

			var pointText = playerPoints[player].GetComponent<TMP_Text>();
			pointText.text  = points.ToString();
			pointText.color = color.Color;
			
			
			if (winner.Item2 < points) {
				winner = (color, points);
			}
		}
		for (int i = players.Count; i < playerPoints.Length; i++)
		{
			playernames[i].SetActive(false);
			playerPoints[i].SetActive(false);
		}

		return winner;
	}
	
	private static int getPointsFromPlayer(PlayerPoints player)
	{
		return player.points;
	}
	
	private void setPlayerColors(GameObject[] playernames, GameObject[] points)
	{
		var players = PlayerManager.Instance.Players;
		for (int i = 0; i < players.Count; i++)
		{
			Color c = players[i].GetComponent<PlayerColor>().GetColor().Color;
			points[i].GetComponent<TMP_Text>().color = c;
			playernames[i].GetComponent<TMP_Text>().color = c;
		}
	}
}
