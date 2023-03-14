using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : GlobalSystem<GameStateManager> {
	public event Action GameStarted = delegate {  }; 
	public event Action GameEnded = delegate {  }; 
	
	[SerializeField] [Scene] private string   _firstScene;
	[SerializeField] [Scene] private string   _uiScene;
	[SerializeField] [Scene] private string   _gameOverScene;
	[SerializeField] [Scene] private string[] _levels;

	private string _currentMainScene;

	public void Start() {
		SceneManager.LoadSceneAsync(_uiScene, LoadSceneMode.Additive);
		ChangeScene(_firstScene);
	}

	public void ChangeScene(string scene) {
		if (_currentMainScene == scene) return;

		if (_currentMainScene != null)
			SceneManager.UnloadSceneAsync(_currentMainScene);

		_currentMainScene = scene;
		SceneManager.LoadSceneAsync(_currentMainScene, LoadSceneMode.Additive);
	}

	public void StartGame(int index) {
		GameStarted();
		ChangeScene(_levels[index<_levels.Length ? index : 0]);
	}

	public void EndGame() {
		GameEnded();
		ChangeScene(_gameOverScene);
	}
}
