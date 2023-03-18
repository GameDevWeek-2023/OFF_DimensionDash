using System;
using UnityEngine;

namespace Player {
	public class PlayerPoints : MonoBehaviour {
		[SerializeField] private int _points_per_coin = 20;
		
		public int points = 0;
		public GameObject coinFX;

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.gameObject.tag == "Item")
			{
				updatePlayerPoints(_points_per_coin);

				GameObject fx = Instantiate(coinFX);
				fx.transform.position = col.transform.position;

				Destroy(col.gameObject);
			}
		}

		public void Reset() {
			points = 0;
		}


		public void updatePlayerPoints(int point)
		{
		
			points = points + point;
		}
	}
}
