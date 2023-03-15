using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dimensions {
	
	public abstract class DimensionDescription : ScriptableObject {
		
		[field:SerializeField]
		public PlayerSpriteType PlayerSprite { get; private set; } = PlayerSpriteType.Base;

		[field:SerializeField]
		public string TileSetName { get; private set; } = null;

		[field:SerializeField]
		public string Name { get; private set; } = "TOOD";

		[field:SerializeField]
		public Image Icon { get; private set; } = null;

		[field:SerializeField]
		public int MaxUsedPerLevel { get; private set; } = -1;

		[field:SerializeField]
		public float MaxTime { get; private set; } = float.MaxValue;

		[field:SerializeField]
		public float MinTime { get; private set; } = 0f;

		public abstract void Apply(List<GameObject>   players);
		public abstract void UnApply(List<GameObject> players);

	}
}
