using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/WaterDimension", order = 0)]
	public class WaterDimension : DimensionDescription {
		[SerializeField] private Vector2 _newGravity;
		[SerializeField] private float   _newJumpHeight      = 2f;
		[SerializeField] private float   _newJumpDuration      = 4f;
		[SerializeField] private float   _accelerationFactor = 0.25f;

		private int   _previousAirJumps;
		private float _maxAcceleration;
		private float _maxDeceleration;
		private float _maxTurnSpeed;
		private float _maxAirAcceleration;
		private float _maxAirDeceleration;
		private float _maxAirTurnSpeed;
		private float _jumpHeight;
		private float _jumpDuration;

		private Vector2 _previousGravity;

		public override void Apply(List<GameObject> players) {
			_previousGravity  = Physics2D.gravity;
			Physics2D.gravity = _newGravity;

			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenPlatformToolkit b)) {
					_previousAirJumps   = b.MaxAirJumps;
					_maxAcceleration    = b.maxAcceleration;
					_maxDeceleration    = b.maxDeceleration;
					_maxTurnSpeed       = b.maxTurnSpeed;
					_maxAirAcceleration = b.maxAirAcceleration;
					_maxAirDeceleration = b.maxAirDeceleration;
					_maxAirTurnSpeed    = b.maxAirTurnSpeed;
					_jumpHeight         = b.jumpHeight;
					_jumpDuration       = b.jumpDuration;

					b.MaxAirJumps        =  -1;
					b.maxAcceleration    *= _accelerationFactor;
					b.maxDeceleration    *= _accelerationFactor;
					b.maxTurnSpeed       *= _accelerationFactor;
					b.maxAirAcceleration *= _accelerationFactor;
					b.maxAirDeceleration *= _accelerationFactor;
					b.maxAirTurnSpeed    *= _accelerationFactor;
					b.jumpHeight         =  _newJumpHeight;
					b.jumpDuration       =  _newJumpDuration;
				}
			}
		}

		public override void UnApply(List<GameObject> players) {
			Physics2D.gravity = _previousGravity;

			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenPlatformToolkit b)) {
					b.MaxAirJumps        = _previousAirJumps;
					b.maxAcceleration    = _maxAcceleration;
					b.maxDeceleration    = _maxDeceleration;
					b.maxTurnSpeed       = _maxTurnSpeed;
					b.maxAirAcceleration = _maxAirAcceleration;
					b.maxAirDeceleration = _maxAirDeceleration;
					b.maxAirTurnSpeed    = _maxAirTurnSpeed;
					b.jumpHeight         = _jumpHeight;
					b.jumpDuration       = _jumpDuration;

					if (p.TryGetComponent(out BoxCollider2D c))
						c.enabled = false;
				}
			}
		}
	}
}
