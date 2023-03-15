using System.Collections.Generic;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/FlappyBirdDimension", order = 0)]
	public class FlappyBirdDimension : DimensionDescription {

		private int _previousAirJumps;
		
		public override void Apply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenPlatformToolkit b)) {
					_previousAirJumps = b.MaxAirJumps;
					b.MaxAirJumps     = -1;
				}
			}
		}

		public override void UnApply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenPlatformToolkit b)) {
					b.MaxAirJumps = _previousAirJumps;
				}
			}
		}
	}
}
