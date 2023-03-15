using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishManager : GlobalSystem<FinishManager>
{
	[SerializeField] private GameStateManager _gameStateManager;

	private void Update()
	{
		
		//player check position
		_gameStateManager.EndGame();
	}
}
