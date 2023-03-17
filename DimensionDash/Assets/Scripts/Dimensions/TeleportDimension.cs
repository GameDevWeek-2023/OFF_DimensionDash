using System.Collections;
using System.Collections.Generic;
using Skripte.Bewegung;
using UnityEngine;

namespace Dimensions
{
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/TeleportDimension", order = 0)]
	public class TeleportDimension : DimensionDescription
	{
		public override void Apply(List<GameObject> players)
		{
			Debug.Log("Teleportdimension apply");
			foreach (GameObject player in players)
			{
				if (player && player.TryGetComponent(out TeleportierenBewegung b))
				{
					b.teleportieren = true;
				}
			}
		}

		public override void UnApply(List<GameObject> players)
		{
			Debug.Log("Teleportdimension unapply");
			foreach (GameObject player in players)
			{
				if (player && player.TryGetComponent(out TeleportierenBewegung b))
				{
					b.teleportieren = false;
				}
			}
		}
	}
}
