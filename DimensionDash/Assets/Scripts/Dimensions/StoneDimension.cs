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
			}
		}

		public override void UnApply(List<GameObject> players)
		{
			foreach(GameObject player in players)
			{
				player.GetComponent<BewegenPlatformToolkit>().enabled = true;
			}
		}
	}
}
