using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	[RequireComponent(typeof(BewegenBasis), typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour {

		[SerializeField] private BewegenBasis _movement;

		private void OnValidate() {
			if (!_movement) _movement = GetComponent<BewegenBasis>();
		}
		private void Awake() {
			if (!_movement) _movement = GetComponent<BewegenBasis>();
		}

		public void OnJump(InputValue input) {
			if (input.isPressed)
				_movement.SpringenStarten();
			else
				_movement.SpringenAbbrechen();
		}

		public void OnMove(InputValue input) {
			_movement.Laufen(input.Get<Vector2>());
		}
	}
}
