using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
	[RequireComponent(typeof(BewegenBasis))]
	public class PlayerController : MonoBehaviour {
		[SerializeField] private BewegenBasis _movement;

		public bool Ready = false;

		private void OnValidate() {
			if (!_movement) _movement = GetComponent<BewegenBasis>();
		}

		private void Awake() {
			if (!_movement) _movement = GetComponent<BewegenBasis>();
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
			_movement.Laufen(input.Get<Vector2>());
		}

		public void OnLeave() {
			if (PlayerManager.Instance.AllowJoining) {
				Destroy(transform.parent.gameObject);
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
