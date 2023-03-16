using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dimensions {
	[CreateAssetMenu(fileName = "DimensionSelection", menuName = "DimensionSelection", order = 0)]
	public class DimensionSelection : ScriptableObject {
		public List<DimensionDescription> Dimensions;
		public List<bool>                 EnabledDimensions;

		private void OnValidate() {
			if (EnabledDimensions.Count > Dimensions.Count)
				EnabledDimensions.RemoveRange(Dimensions.Count, EnabledDimensions.Count - Dimensions.Count);

			while (EnabledDimensions.Count < Dimensions.Count)
				EnabledDimensions.Add(true);
		}

		public List<DimensionDescription> GetEnabledDimensions() {
			var ret = new List<DimensionDescription>();
			ret.Capacity = Dimensions.Count;
			for (var i = 0; i < Dimensions.Count; ++i) {
				if (i < EnabledDimensions.Count && EnabledDimensions[i])
					ret.Add(Dimensions[i]);
			}

			return ret;
		}
	}
}
