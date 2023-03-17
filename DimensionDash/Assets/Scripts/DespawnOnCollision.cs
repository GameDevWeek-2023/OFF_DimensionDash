using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DespawnOnCollision : MonoBehaviour {
	private void OnCollisionEnter2D(Collision2D col) {
		Destroy(gameObject, 0.5f);
	}
}
