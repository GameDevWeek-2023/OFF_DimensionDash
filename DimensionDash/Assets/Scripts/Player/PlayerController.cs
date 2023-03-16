using System;
using Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	public class PlayerController : MonoBehaviour {
		[SerializeField] private BewegenBasis _movement;

		private Vector2 _direction = Vector2.zero;
		
		public bool Ready = false;

		private void OnValidate() {
			if (!_movement) _movement = GetComponentInChildren<BewegenBasis>();
		}

		private void Awake() {
			if (!_movement) _movement = GetComponentInChildren<BewegenBasis>();
		}

		private void Update() {
			_movement.Laufen(_direction);
		}

		private void OnDisable() {
			_movement.Laufen(Vector2.zero);
		}

		public void OnJump(InputValue input) {
			if (input.isPressed)
				_movement.SpringenStarten();
			else
				_movement.SpringenAbbrechen();
		}

		public void OnMove(InputValue input) {
			_direction = input.Get<Vector2>();
			_movement.Laufen(input.Get<Vector2>());
		}

		public void OnLeave() {
			if (PlayerManager.Instance.AllowJoining) {
				Destroy(gameObject);
				Debug.Log("Player left");
			}
		}

		public void OnReady() {
			Ready = true;
		}

		public void OnMenuLeft() {
			// TODO
		}
		public void OnMenuRight() {
			// TODO
		}
	}
}
