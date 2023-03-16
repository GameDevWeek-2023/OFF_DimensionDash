using System.Collections;
using System.Collections.Generic;
using Movement;
using Player;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/StoneDimension", order = 0)]
	public class StoneDimension : DimensionDescription {
		[SerializeField] private PhysicsMaterial2D _newPhysicsMaterial;

		private PhysicsMaterial2D _physicsMaterial;

		public override void Apply(List<GameObject> players) {
			foreach (GameObject player in players) {
				player.GetComponent<BewegenPlatformToolkit>().enabled = false;
				var body = player.GetComponent<Rigidbody2D>();
				body.velocity    = Vector2.zero;
				_physicsMaterial = body.sharedMaterial;
				if (_newPhysicsMaterial)
					body.sharedMaterial = _newPhysicsMaterial;
				player.GetComponent<Juice>().squashAndStretch = false;
			}
		}

		public override void UnApply(List<GameObject> players) {
			foreach (GameObject player in players) {
				player.GetComponent<BewegenPlatformToolkit>().enabled = true;
				player.GetComponent<Juice>().squashAndStretch         = true;
				var body = player.GetComponent<Rigidbody2D>();
				body.sharedMaterial = _physicsMaterial;
			}
		}
	}
}
