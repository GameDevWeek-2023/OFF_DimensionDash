using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
	private Animator   _cameraMovement;
	void Start()
    {
	    _cameraMovement = this.gameObject.GetComponent<Animator>();
    }
	
    void Update()
    {
	    if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End"))
	    {
		    //TODO: Entering GameEndScreen
		    SceneManager.LoadScene("EndScene");
	    }
    }
}
