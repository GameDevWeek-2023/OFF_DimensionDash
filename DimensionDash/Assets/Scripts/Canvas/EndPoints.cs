using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using TMPro;
using UnityEngine;

public class EndPoints : MonoBehaviour
{
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
	void Start()
	{
		GameObject[] playerPoints      = new[] { player1, player2, player3, player4, player5, player6, player7, player8 };
		GameObject[] playernames = new[] {player1_Points, player2_Points, player3_Points, player4_Points, player5_Points, player6_Points, player7_Points, player8_Points};
		GameObject[] players     = CanvasPoints._players;
		int winnerNumber = changePlayerStats(players, playernames, playerPoints);
		setPlayerColors(players, playernames, playerPoints);
		setWinnerText(winnerNumber);
	    foreach (GameObject player in players)
	    {
		    Destroy(player);
	    }
	}

	private void setWinnerText(int playerNumber)
	{
		winnerText.GetComponent<TMP_Text>().text = "Player " + playerNumber + " wins";
	}
	
	private int changePlayerStats(GameObject[] players, GameObject[] playernames, GameObject[] playerPoints)
	{
		int[] winner = new int[] {0,0};
		for (int player = 0; player < players.Length; player++)
		{
			playerPoints[player].SetActive(true);
			playernames[player].SetActive(true);
			int points = getPointsFromPlayer(players[player].GetComponent<PlayerPoints>());
			playerPoints[player].GetComponent<TMP_Text>().text = points.ToString();
			if (winner[1] < points)
			{
				winner[0] = player+1;
				winner[1] = points;
			}
		}
		for (int i = players.Length; i < playerPoints.Length; i++)
		{
			playernames[i].SetActive(false);
			playerPoints[i].SetActive(false);
		}

		return winner[0];
	}
	
	private static int getPointsFromPlayer(PlayerPoints player)
	{
		return player.points;
	}
	
	private void setPlayerColors(GameObject[] players, GameObject[] playernames, GameObject[] points)
	{
		for (int i = 0; i < players.Length; i++)
		{
			Color c = players[i].GetComponent<PlayerColor>().GetColor();
			points[i].GetComponent<TMP_Text>().color = c;
			playernames[i].GetComponent<TMP_Text>().color = c;
		}
	}
}
