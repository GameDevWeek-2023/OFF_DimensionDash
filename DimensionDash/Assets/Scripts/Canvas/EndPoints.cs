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
	                   player8_Points;
	void Start()
	{
		GameObject[] points      = new[] { player1, player2, player3, player4, player5, player6, player7, player8 };
		GameObject[] playernames = new[] {player1_Points, player2_Points, player3_Points, player4_Points, player5_Points, player6_Points, player7_Points, player8_Points};
		GameObject[] players     = CanvasPoints._players;
		changePlayerStats(players, playernames, points);
		setPlayerColors(players, playernames, points);
	    foreach (GameObject player in players)
	    {
		    Destroy(player);
	    }
	    
	}

	private void changePlayerStats(GameObject[] players, GameObject[] playernames, GameObject[] points)
	{
		Debug.Log(players.Length + " " + points.Length);
		for (int i = 0; i < players.Length; i++)
		{
			points[i].SetActive(true);
			playernames[i].SetActive(true);
			Debug.Log(getPointsFromPlayer(players[i].GetComponent<PlayerPoints>()));
			points[i].GetComponent<TMP_Text>().text = getPointsFromPlayer(players[i].GetComponent<PlayerPoints>());
		}
		for (int i = players.Length; i < points.Length; i++)
		{
			playernames[i].SetActive(false);
			points[i].SetActive(false);
		}
	}
	
	private static string getPointsFromPlayer(PlayerPoints player)
	{
		return player.points.ToString();
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
