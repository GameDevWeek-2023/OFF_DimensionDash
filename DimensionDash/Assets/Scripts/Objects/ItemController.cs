using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemController : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D col)
    {
	    if (col.tag == "Player")
	    {
		    Destroy(this.gameObject);
		    //TODO: Player Points +1
	    }
    }
}
