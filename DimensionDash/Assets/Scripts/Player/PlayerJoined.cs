using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoined : MonoBehaviour
{
	[SerializeField]
	private GameObject joinG;

	[SerializeField]
	private GameObject colorSelectG;

	void Awake()
    {
		joinG.SetActive(true);
		colorSelectG.SetActive(false);
	}


	public void PlayerJoinedMe() 
	{
		joinG.SetActive(false);
		colorSelectG.SetActive(true);
	}
}
