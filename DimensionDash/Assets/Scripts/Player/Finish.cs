using UnityEngine;

namespace Player {
	public class Finish : MonoBehaviour {
		private void OnTriggerEnter2D(Collider2D col) {
			if (col.gameObject.TryGetComponent(out PlayerPoints points)) {
				points.points += 5;
				GameStateManager.Instance.EndGame();
				Destroy(this);
			}
		}
	}
}
