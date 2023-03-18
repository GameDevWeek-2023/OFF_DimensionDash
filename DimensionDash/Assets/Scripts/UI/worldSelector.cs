using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldSelector : MonoBehaviour
{
	[SerializeField]
	private GameObject pauseManager;

	private void Awake() {
		pauseManager = GameObject.Find("Pause Menu");
	}

	public void OnMouseDown() {
		pauseManager.GetComponent<PauseMenuHandler>().OpenWorldSelect();
	}
}
