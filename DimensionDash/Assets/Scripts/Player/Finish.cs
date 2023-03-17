using UnityEngine;

namespace Player {
	public class Finish : MonoBehaviour {
		[SerializeField] private int _points = 100;
		private void OnTriggerEnter2D(Collider2D col) {
			var cam = Camera.main;
			if (cam) {
				// don't finish the game, if the object isn't visible yet
				var camRelativePosition = transform.position - cam.transform.position;
				var camSize             = new Vector2(cam.aspect, 1f) * cam.orthographicSize;
				if (Mathf.Abs(camRelativePosition.x) > camSize.x || Mathf.Abs(camRelativePosition.y) > camSize.y)
					return;
			}
			
			if (col.gameObject.TryGetComponent(out PlayerPoints points)) {
				points.updatePlayerPoints(_points);
				GameStateManager.Instance.EndGameAfter(4f);
				Destroy(gameObject);
			}
		}
	}
}
