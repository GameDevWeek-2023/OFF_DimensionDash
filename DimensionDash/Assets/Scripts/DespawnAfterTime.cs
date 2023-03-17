using UnityEngine;

public class DespawnAfterTime : MonoBehaviour {
	[SerializeField] private float _delay = 8f;
	private void Start() {
		Destroy(gameObject, _delay);
	}
}
