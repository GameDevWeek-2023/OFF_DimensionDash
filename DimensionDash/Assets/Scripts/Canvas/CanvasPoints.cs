using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPoints : MonoBehaviour
{
	public static readonly List<GameObject> _playerText = new List<GameObject>();
	public static    GameObject[]     _players;
	private          float            _time                 = 0;
	private          float            _secondsPerDimension  = ObjectManager.SecondsPerDimension;
	private readonly float            _secondsCompleteLevel = 50f;	//TODO: Specify complete Level time
	private          Slider           _levelSlider;
	private          TMP_Text         _dimensionCountdown;
	private void Start()
	{
		initializePlayerPointsCanvas(this.gameObject);
		_players              = GameObject.FindGameObjectsWithTag("Player");                          //SerializeField
		_levelSlider          = gameObject.transform.Find("LevelProgressBar").GetComponent<Slider>(); //SerializeField
		_levelSlider.maxValue = _secondsCompleteLevel;
		_dimensionCountdown   = gameObject.transform.Find("DimensionCountdown").GetComponent<TMP_Text>(); //SerializeField
		_secondsPerDimension  = ObjectManager.SecondsPerDimension;

		foreach (GameObject player in _players)
		{
			DontDestroyOnLoad(player);
		}
		
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
	    
	    _time += Time.deltaTime;
	    float levelProgress = _time % _secondsCompleteLevel;
	    _levelSlider.value = levelProgress;
	    float dimensionProgress = (float) Math.Round(Math.Abs((_time % _secondsPerDimension)-_secondsPerDimension),2);
	    _dimensionCountdown.text = dimensionProgress.ToString();

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
