using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/FlappyBirdDimension", order = 0)]
	public class FlappyBirdDimension : DimensionDescription {

		private int   _previousAirJumps;
		private float _maxAcceleration;
		private float _maxDeceleration;
		private float _maxTurnSpeed;
		
		public override void Apply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenPlatformToolkit b)) {
					_previousAirJumps = b.MaxAirJumps;
					_maxAcceleration  = b.maxAcceleration;
					_maxDeceleration  = b.maxDeceleration;
					_maxTurnSpeed     = b.maxTurnSpeed;
					
					b.MaxAirJumps     = -1;
					b.maxAcceleration = 0;
					b.maxDeceleration = 0;
					b.maxTurnSpeed    = 0;
				}
			}
		}

		public override void UnApply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenPlatformToolkit b)) {
					b.MaxAirJumps     = _previousAirJumps;
					b.maxAcceleration = _maxAcceleration;
					b.maxDeceleration = _maxDeceleration;
					b.maxTurnSpeed    = _maxTurnSpeed;
				}
			}
		}
	}
}
