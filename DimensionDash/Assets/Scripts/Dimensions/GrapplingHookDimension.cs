using System.Collections;
using System.Collections.Generic;
using Skripte.Bewegung;
using UnityEngine;

namespace Dimensions
{
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/GrapplingHookDimension", order = 0)]
	public class GrapplingHookDimension : DimensionDescription
	{
		public override void Apply(List<GameObject> players)
		{
			foreach (GameObject player in players)
			{
				if (player && player.TryGetComponent(out GrapplingHookBewegung b))
				{
					b.grapplinghook = true;
				}
			}
		}

		public override void UnApply(List<GameObject> players)
		{
			foreach (GameObject player in players)
			{
				if (player && player.TryGetComponent(out GrapplingHookBewegung b))
				{
					b.grapplinghook = false;
				}
			}
		}
	}
}
