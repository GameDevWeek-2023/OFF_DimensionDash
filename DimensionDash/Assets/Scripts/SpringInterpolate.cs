using System;
using UnityEngine;

public class SpringInterpolateParent : MonoBehaviour {
	public GameObject Child;

	private void OnEnable() {
		if(Child)
			Child.SetActive(true);
	}
	private void OnDisable() {
		if(Child)
			Child.SetActive(false);
	}
}

public class SpringInterpolate : MonoBehaviour {

	[SerializeField] private float _springConstant = 2;
	[SerializeField] private float _damping        = 10;
	[SerializeField] private float _mass           = 1;
	[SerializeField] private float _maxDistance           = 2f;
	
	private Transform _orgParent;
	private Vector2   _velocity;

	private void Start() {
		_orgParent = transform.parent;
		transform.SetParent(null);
		_orgParent.gameObject.AddComponent<SpringInterpolateParent>().Child = gameObject;
	}

	private void OnEnable() {
		if (_orgParent) {
			_velocity          = new Vector2(0, 2f);
			transform.position = _orgParent.position;
		}
	}

	private void Update() {
		if (!_orgParent) {
			Destroy(gameObject);
			return;
		}

		var diff = new Vector2(transform.position.x - _orgParent.position.x, transform.position.y - _orgParent.position.y);

		if (diff.magnitude > _maxDistance) {
			diff               = diff.normalized * _maxDistance;
			transform.position = _orgParent.position + new Vector3(diff.x, diff.y, 0f);
		}

		var force = -_springConstant * diff + Physics2D.gravity*_mass - _velocity * _damping;
		_velocity += force * (_mass * Time.deltaTime);
		
		transform.position += new Vector3(_velocity.x*Time.deltaTime, _velocity.y*Time.deltaTime, 0f);
	}

}
