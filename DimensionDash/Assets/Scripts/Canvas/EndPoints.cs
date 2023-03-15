using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EndPoints : MonoBehaviour
{
	[SerializeField] private GameObject player1, player2, player3, player4, player1_Points, player2_Points, player3_Points, player4_Points;
	void Start()
	{
		GameObject[] points      = new[] { player1, player2, player3, player4 };
		GameObject[] playernames = new[] {player1_Points, player2_Points, player3_Points, player4_Points};
		GameObject[] players     = CanvasPoints._players;
		changePlayerStats(players, playernames, points);
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
}
