using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasPoints : MonoBehaviour
{
	private List<GameObject> _playerText = new List<GameObject>();
	private static GameObject[] _players;
	private void Start()
	{
		initializePlayerPointsCanvas(this.gameObject);
		_players = GameObject.FindGameObjectsWithTag("Player");
		
		for (int i = 0; i < _players.Length; i++)
		{
			_playerText[i].SetActive(true);;
		}

		for (int i = _players.Length; i < _playerText.Count; i++)
		{
			_playerText[i].SetActive(false);
		}
	}

	void Update()
    {
	    for (int i = 0; i < _players.Length; i++)
	    {
		    _playerText[i].GetComponent<TMP_Text>().text = getPointsFromPlayer(_players[i].GetComponent<PlayerPoints>());
	    }
    }

	private static string getPointsFromPlayer(PlayerPoints player)
	{
		return player.points.ToString();
	}

	private void initializePlayerPointsCanvas(GameObject parent)
    {
	    List<GameObject> parents     = new List<GameObject>();
	    Transform[]      allChildren = parent.GetComponentsInChildren<Transform>();
	    foreach (Transform obj in allChildren)
	    {
		    if (obj.transform.parent == parent.transform && obj.name.Contains("Points"))
		    {
			    _playerText.Add(obj.gameObject);
		    }
	    }
    }
}
