using UnityEngine;
using UnityEngine.UI;

namespace Dimensions {

	public enum Dimension {
		Base,
		Vector
	}
	
	public abstract class DimensionDescription : ScriptableObject {
		
		[field:SerializeField]
		public Dimension Dimension { get; private set; } = Dimension.Base;

		[field:SerializeField]
		public string TileSetName { get; private set; } = null;

		[field:SerializeField]
		public string Name { get; private set; } = "TOOD";

		[field:SerializeField]
		public Image Icon { get; private set; } = null;

		public abstract void Apply();
		public abstract void UnApply();

	}
}
