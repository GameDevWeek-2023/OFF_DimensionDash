using System.Collections;
using System.Collections.Generic;
using Movement;
using Player;
using UnityEngine;

namespace Dimensions
{
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/StoneDimension", order = 0)]
	public class StoneDimension : DimensionDescription
	{
		public override void Apply(List<GameObject> players)
		{
			foreach(GameObject player in players)
			{
				player.GetComponent<BewegenPlatformToolkit>().enabled = false;
				player.GetComponent<Rigidbody2D>().velocity           = Vector2.zero;
				player.GetComponent<Juice>().squashAndStretch         = false;
			}
		}

		public override void UnApply(List<GameObject> players)
		{
			foreach(GameObject player in players)
			{
				player.GetComponent<BewegenPlatformToolkit>().enabled = true;
				player.GetComponent<Juice>().squashAndStretch         = true;
			}
		}
	}
}
