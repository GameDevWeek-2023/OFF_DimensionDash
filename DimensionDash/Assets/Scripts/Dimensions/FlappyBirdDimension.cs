using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/FlappyBirdDimension", order = 0)]
	public class FlappyBirdDimension : DimensionDescription {
		[SerializeField] private bool  _unlockRotation = false;
		[SerializeField] private float _newJumpHeight;

		private int   _previousAirJumps;
		private float _maxAcceleration;
		private float _maxDeceleration;
		private float _maxTurnSpeed;
		private float _jumpHeight;

		public override void Apply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenPlatformToolkit b)) {
					_previousAirJumps = b.MaxAirJumps;
					_maxAcceleration  = b.maxAcceleration;
					_maxDeceleration  = b.maxDeceleration;
					_maxTurnSpeed     = b.maxTurnSpeed;
					_jumpHeight       = b.jumpHeight;

					b.MaxAirJumps     = -1;
					b.maxAcceleration = 0;
					b.maxDeceleration = 0;
					b.maxTurnSpeed    = 0;
					b.jumpHeight      = _newJumpHeight;

					if (_unlockRotation) {
						var body = p.GetComponent<Rigidbody2D>();
						body.constraints     = RigidbodyConstraints2D.None;
						body.angularVelocity = Random.Range(-360f, 360f);

						if (p.TryGetComponent(out BoxCollider2D c))
							c.enabled = true;
					}
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
					b.jumpHeight      = _jumpHeight;

					if (_unlockRotation) {
						var body = p.GetComponent<Rigidbody2D>();
						body.constraints = RigidbodyConstraints2D.FreezeRotation;
						body.rotation    = 0;
					}

					if (p.TryGetComponent(out BoxCollider2D c))
						c.enabled = false;
				}
			}
		}
	}
}
