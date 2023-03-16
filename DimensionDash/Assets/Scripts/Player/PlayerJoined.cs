using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoined : MonoBehaviour
{
	[SerializeField]
	private GameObject joinG;

	[SerializeField]
	private GameObject colorSelectG;

	[SerializeField]
	private GameObject startIndicator1;

	private bool iAmReady;


	void Awake()
    {
		joinG.SetActive(true);
		colorSelectG.SetActive(false);
		startIndicator1.SetActive(false);
	}


	public void PlayerJoinedMe() 
	{
		joinG.SetActive(false);
		colorSelectG.SetActive(true);
	}

	void Update() {
		if (iAmReady && !startIndicator1.activeSelf)
			startIndicator1.SetActive(true);
		else if (!iAmReady && startIndicator1.activeSelf)
			startIndicator1.SetActive(false);
	}
}
