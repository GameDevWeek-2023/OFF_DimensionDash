using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
	[SerializeField]
	private GameObject pauseScreenG;
	[SerializeField]
	private GameObject creditsScreenG;
	[SerializeField]
	private GameObject controlsScreenG;
	[SerializeField]
	private GameObject WorldSelect;
	[SerializeField]
	private GameObject pauseMenu;
	[SerializeField]
	private GameObject greyScreen;
	[SerializeField]
	private Text timerText;
	[SerializeField]
	private float timerTime;

	private float timerTimer;

	private bool timeStop;

	public enum CurrentUI {
		pauseScreen,
		creditsScreen,
		controlsScreen
	}

	public CurrentUI currentUI;


	private void Start() {
		pauseMenu.SetActive(false);
	}



	private void Update() {

		if (Input.GetKeyDown(KeyCode.Escape) && !timeStop) {
			timeStop = true;
			OpenPauseMenu();
			Time.timeScale = 0;
		}

		if (timerTimer > 0) {
			timerTimer -= Time.unscaledDeltaTime;
			int timeLeft = (int) timerTimer;
			timerText.text = timerTimer.ToString("0.0");
			if(timerTimer <= 0) {
				timerTimer = 0;
				timerText.gameObject.SetActive(false);
				timeStop = false;
				Time.timeScale = 1;
			}
		}
	}


	public void BackInUI() {
		switch (currentUI) {
			case CurrentUI.creditsScreen:
				CloseCredits();
				break;
			case CurrentUI.controlsScreen:
				CloseControls();
				break;

			default:
				break;
		}
	}


	public void OpenWorldSelect() {
		timeStop = true;
		Time.timeScale = 0;
		greyScreen.SetActive(true);
		WorldSelect.SetActive(true);
	}

	public void CloseWorldScreen() {
		timeStop = false;
		Time.timeScale = 1;
		greyScreen.SetActive(false);
		WorldSelect.SetActive(false);
	}



	public void RestartGame() {
		GameStateManager.Instance.ChangeToInitialScreen();

	}


	public void ExitGame() {
		Debug.Log("2");
		Application.Quit();
	}


	public void ExitPauseScreen() {
		greyScreen.SetActive(false);
		timerTimer = timerTime;
		timerText.gameObject.SetActive(true);
		pauseScreenG.SetActive(true);
		pauseMenu.SetActive(false);
		creditsScreenG.SetActive(false);
		controlsScreenG.SetActive(false);
	}


	public void OpenPauseMenu() {
		greyScreen.SetActive(true);
		timerText.gameObject.SetActive(false);
		pauseMenu.SetActive(true);
		currentUI = CurrentUI.pauseScreen;
		pauseScreenG.SetActive(true);
		creditsScreenG.SetActive(false);
		controlsScreenG.SetActive(false);
	}

	public void OpenCredits() {
		currentUI = CurrentUI.creditsScreen;
		pauseScreenG.SetActive(false);
		creditsScreenG.SetActive(true);
	}

	public void OpenControls() {
		currentUI = CurrentUI.controlsScreen;
		pauseScreenG.SetActive(false);
		controlsScreenG.SetActive(true);

	}

	public void CloseCredits() {
		currentUI = CurrentUI.pauseScreen;
		pauseScreenG.SetActive(true);
		creditsScreenG.SetActive(false);
	}

	public void CloseControls() {
		currentUI = CurrentUI.pauseScreen;
		pauseScreenG.SetActive(true);
		controlsScreenG.SetActive(false);
	}

}
