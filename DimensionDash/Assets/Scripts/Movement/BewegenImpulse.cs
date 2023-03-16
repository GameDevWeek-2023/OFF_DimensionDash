using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Movement {
	public class BewegenImpulse : BewegenÜberschreiben {
		[SerializeField] private float _impulse = 20f;
		
		[SerializeField] private Rigidbody2D  _body;
		[SerializeField] private BewegenBasis _basis;
		
		private Vector2      _direction;

		private void OnValidate() {
			_body  = GetComponent<Rigidbody2D>();
			_basis = GetComponent<BewegenPlatformToolkit>();
		}

		private void OnEnable() {
			_basis.Laufen(Vector2.zero);
			_body.constraints = RigidbodyConstraints2D.None;
		}

		private void OnDisable() {
			_body.constraints = RigidbodyConstraints2D.FreezeRotation;
			_body.rotation    = 0f;
		}

		public override bool WennLaufen(Vector2 richtung) {
			if (richtung.sqrMagnitude <= 0.001f)
				return true;
			
			_direction = richtung.normalized;
			return false;
		}

		public override bool WennSpringen() {
			_body.velocity = Vector2.zero;
			_body.AddForce(_direction * _impulse, ForceMode2D.Impulse);
			_body.AddTorque(Random.Range(-360f, 360f), ForceMode2D.Impulse);
			return false;
		}

		private void Update() {
			// required to have enable
		}
	}
}
