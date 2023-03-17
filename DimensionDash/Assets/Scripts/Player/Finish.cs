using UnityEngine;

namespace Player {
	public class Finish : MonoBehaviour {
		[SerializeField] private int _points = 100;
		private void OnTriggerEnter2D(Collider2D col) {
			if (col.gameObject.TryGetComponent(out PlayerPoints points)) {
				points.updatePlayerPoints(_points);
				GameStateManager.Instance.EndGameAfter(4f);
				Destroy(gameObject);
			}
		}
	}
}
