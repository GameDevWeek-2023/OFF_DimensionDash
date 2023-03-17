using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "Dimension", menuName = "Dimensions/AstroidsDimension", order = 0)]
	public class AstroidsDimension : DimensionDescription {

		public override void Apply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenAstroids b)) {
					b.enabled = true;
				}
			}
		}

		public override void UnApply(List<GameObject> players) {
			foreach (var p in players) {
				if (p && p.TryGetComponent(out BewegenAstroids b)) {
					b.enabled = false;
				}
			}
		}
	}
}
