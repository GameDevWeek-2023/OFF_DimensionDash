using System;
using UnityEngine;

public class SpringInterpolate : MonoBehaviour {

	[SerializeField] private float _springConstant = 2;
	[SerializeField] private float _damping        = 10;
	[SerializeField] private float _mass        = 1;
	
	private Transform _orgParent;
	private Vector2   _velocity;

	private void Start() {
		_orgParent = transform.parent;
		transform.SetParent(null);
	}

	private void Update() {
		if (!_orgParent) {
			Destroy(gameObject);
			return;
		}

		var diff = new Vector2(transform.position.x - _orgParent.position.x, transform.position.y - _orgParent.position.y);
		
		var force = -_springConstant * diff + Physics2D.gravity*_mass - _velocity * _damping;
		_velocity += force * (_mass * Time.deltaTime);
		
		transform.position += new Vector3(_velocity.x*Time.deltaTime, _velocity.y*Time.deltaTime, 0f);
	}

}
