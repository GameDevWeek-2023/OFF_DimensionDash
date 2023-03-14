using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
	public int   points = 0;
	
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Item")
		{
			Destroy(col.gameObject);
			updatePlayerPoints(1);
		}
	}

	public void updatePlayerPoints(int point)
    {
		
	    points = points + point;
	    if (points < 1)
	    {
		    //TODO: Player lose
	    }
    }
}
