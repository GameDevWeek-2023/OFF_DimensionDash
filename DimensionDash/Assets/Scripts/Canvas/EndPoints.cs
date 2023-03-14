using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EndPoints : MonoBehaviour
{
	[SerializeField] private GameObject player1, player2, player3, player4;
	void Start()
	{
		GameObject[] points  = new[] {player1, player2, player3, player4};
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); //SerializeField
	    changePlayerStats(players, points);

	    /*foreach (GameObject player in players)
	    {
		    Destroy(player);
	    }*/
	}

	private void changePlayerStats(GameObject[] players, GameObject[] points)
	{
		Debug.Log(players.Length + " " + points.Length);
		for (int i = 0; i < players.Length; i++)
		{
			points[i].SetActive(true);
			Debug.Log(getPointsFromPlayer(players[i].GetComponent<PlayerPoints>()));
			points[i].GetComponent<TMP_Text>().text = getPointsFromPlayer(players[i].GetComponent<PlayerPoints>());
		}
		for (int i = players.Length; i < points.Length; i++)
		{
			points[i].SetActive(false);
		}
	}
	
	private static string getPointsFromPlayer(PlayerPoints player)
	{
		return player.points.ToString();
	}
}
