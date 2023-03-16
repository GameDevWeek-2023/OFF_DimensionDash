using System;
using System.Collections;
using CookAGeddon.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : GlobalSystem<GameStateManager> {
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
		if (_currentMainScene == scene)
			return;

		StartCoroutine(ChangeSceneCo(scene));
	}

	public IEnumerator ChangeSceneCo(string scene) {
		var last = _currentMainScene;
		_currentMainScene = scene;
		
		foreach (var fade in FindObjectsOfType<CameraFade>()) {
			fade.FadeOut(new CameraFade.Options(){FadeTime = 1f});
		}
		yield return new WaitForSecondsRealtime(1f);

		if (last != null) {
			yield return SceneManager.UnloadSceneAsync(last);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		
		yield return SceneManager.LoadSceneAsync(_currentMainScene, LoadSceneMode.Additive);
		
		foreach (var fade in FindObjectsOfType<CameraFade>()) {
			fade.FadeIn();
		}
	}

	public void StartGame(int index) {
		ChangeScene(_levels[index<_levels.Length ? index : 0]);
	}

	public void EndGame() {
		ChangeScene(_gameOverScene);
	}
}
