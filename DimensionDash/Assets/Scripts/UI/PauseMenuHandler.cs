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
	private GameObject pauseMenu;
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



	public void RestartGame() {

	}


	public void ExitGame() {

	}


	public void ExitPauseScreen() {
		timerTimer = timerTime;
		timerText.gameObject.SetActive(true);
		pauseScreenG.SetActive(true);
		pauseMenu.SetActive(false);
		creditsScreenG.SetActive(false);
		controlsScreenG.SetActive(false);
	}


	public void OpenPauseMenu() {
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
