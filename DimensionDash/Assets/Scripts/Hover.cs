using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hover : MonoBehaviour {
	private Vector3 _initialPosition;
	private float   _offset;

	private void Start() {
		_initialPosition = transform.position;
		_offset          = Random.Range(-1f, 1f);
	}

	private void Update() {
		transform.position = _initialPosition + Mathf.Sin(_offset+Time.time * 4f) * 0.2f * Vector3.up;
	}
}
