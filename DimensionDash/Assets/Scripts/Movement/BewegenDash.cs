using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Movement {
	public class BewegenDash : BewegenÜberschreiben {
		[SerializeField] private float _impulse = 1000f;
		
		[SerializeField] private Rigidbody2D  _body;
		
		private Vector2      _direction;

		private void OnValidate() {
			_body  = GetComponent<Rigidbody2D>();
		}

		public override bool WennLaufen(Vector2 richtung) {
			if (richtung.sqrMagnitude > 0.01f)
				_direction = richtung.normalized;
			
			return true;
		}

		public override bool WennSpringen() {
			_body.velocity = Vector2.zero;
			_body.AddForce(_direction * _impulse, ForceMode2D.Impulse);
			return false;
		}

		private void Update() {
			// required to have enable
		}
	}
}
