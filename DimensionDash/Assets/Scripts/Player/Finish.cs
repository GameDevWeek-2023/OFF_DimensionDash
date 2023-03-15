using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Finish : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			col.gameObject.GetComponent<PlayerPoints>().points += 5;
			GameObject.Find("input_system").GetComponent<PlayerManager>()._gameStateManager.EndGame();
			Destroy(this);
		}
	}
}
