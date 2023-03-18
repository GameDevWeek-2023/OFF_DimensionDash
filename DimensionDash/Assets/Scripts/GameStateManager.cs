using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : GlobalSystem<GameStateManager> {
	[SerializeField] [Scene] private string   _firstScene;
	[SerializeField] [Scene] private string   _uiScene;
	[SerializeField] [Scene] private string   _gameOverScene;
	[SerializeField] [Scene] private string[] _levels;

	private string _currentMainScene;
	private bool   _loading = false;

	public void Start() {
		SceneManager.LoadSceneAsync(_uiScene, LoadSceneMode.Additive);
		ChangeScene(_firstScene);
	}

	public void ChangeScene(string scene) {
		if (_currentMainScene == scene || _loading)
			return;

		StartCoroutine(ChangeSceneCo(scene, 1f));
	}

	private IEnumerator ChangeSceneCo(string scene, float fadeOutDuration) {
		_loading = true;
		var last = _currentMainScene;
		_currentMainScene = scene;
		
		foreach (var fade in FindObjectsOfType<CameraFade>()) {
			fade.FadeOut(new CameraFade.Options(){FadeTime = fadeOutDuration});
		}
		yield return new WaitForSecondsRealtime(fadeOutDuration);
		Time.timeScale = 1f;

		if (last != null) {
			yield return SceneManager.UnloadSceneAsync(last);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		
		yield return SceneManager.LoadSceneAsync(_currentMainScene, LoadSceneMode.Additive);
		
		foreach (var fade in FindObjectsOfType<CameraFade>()) {
			fade.FadeIn();
		}

		_loading = false;
	}

	public void StartGame(int index) {
		ChangeScene(_levels[index<_levels.Length ? index : 0]);
	}

	public void EndGame() {
		ChangeScene(_gameOverScene);
	}
	public void EndGameAfter(float delay) {
		if (_loading)
			return;
		
		StartCoroutine(EndGameAfterCo(delay));
	}
	public void ChangeToInitialScreen() {
		ChangeScene(_firstScene);
	}

	private IEnumerator EndGameAfterCo(float delay) {
		_loading       = true;
		Time.timeScale = 0.1f;
		yield return new WaitForSecondsRealtime(delay/2f);
		yield return ChangeSceneCo(_gameOverScene, delay/2f);
	}
}
