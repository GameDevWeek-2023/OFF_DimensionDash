using System.Collections.Generic;
using UnityEngine;

namespace Player {
	[CreateAssetMenu(fileName = "NamePool", menuName = "NamePool", order = 0)]
	public class PlayerNamePool : ScriptableObject {

		public List<string> Names;
		
	}
}
