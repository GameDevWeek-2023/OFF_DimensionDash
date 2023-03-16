using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/CubeDimension", order = 0)]
	public class CubeDimension : DimensionDescription {

		public override void Apply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenImpulse b)) {
					b.enabled = true;

					if (p.TryGetComponent(out BoxCollider2D c))
							c.enabled = true;
				}
			}
		}

		public override void UnApply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenImpulse b)) {
					b.enabled = false;

					if (p.TryGetComponent(out BoxCollider2D c))
						c.enabled = false;
				}
			}
		}
	}
}
