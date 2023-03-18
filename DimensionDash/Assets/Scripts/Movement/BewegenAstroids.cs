using System;
using Player;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Movement {
	public class BewegenAstroids : BewegenÜberschreiben {
		[SerializeField] private float      _flyForce       = 400f;
		[SerializeField] private float      _flyMaxVelocity = 100f;
		[SerializeField] private float      _flyDrag        = 0.2f;
		[SerializeField] private GameObject _bulletPrefab;
		[SerializeField] private float      _bulletVelocity = 100f;
		[SerializeField] private float      _recoilImpulse  = 100f;
		[SerializeField] private Transform  _shootPosition;

		[SerializeField] private Rigidbody2D  _body;
		[SerializeField] private BewegenBasis _basis;

		private ObjectPool<GameObject> _bulletPool;

		private float _orgDrag         = 0f;
		private float _orgGravityScale = 10f;

		private Vector2 _direction = Vector2.up;
		private float   _magnitude = 0f;

		private void OnValidate() {
			_body  = GetComponent<Rigidbody2D>();
			_basis = GetComponent<BewegenPlatformToolkit>();
		}

		private void Awake() {
			var color = GetComponent<PlayerColor>().GetColor().Color;
			_bulletPool = new ObjectPool<GameObject>(() => {
				var go = Instantiate(_bulletPrefab, transform);
				go.GetComponentInChildren<SpriteRenderer>().color = color;
				return go;
			}, (obj) => obj.SetActive(true), obj => obj.SetActive(false), Destroy, false, 4, 16);
		}

		private void OnEnable() {
			_basis.Laufen(Vector2.zero);
			_body.constraints  = RigidbodyConstraints2D.None;
			_orgGravityScale   = _body.gravityScale;
			_body.gravityScale = 0f;
			_body.velocity     = Vector2.zero;
			_orgDrag           = _body.drag;
			_body.drag         = _flyDrag;
		}

		private void OnDisable() {
			_body.constraints  = RigidbodyConstraints2D.FreezeRotation;
			_body.gravityScale = _orgGravityScale;
			_body.rotation     = 0f;
			_body.drag         = _orgDrag;
		}

		public override bool WennAktuallisieren() {
			_body.MoveRotation(Vector2.SignedAngle(Vector2.right, _direction));
			_body.AddForce(_magnitude * _direction * _flyForce, ForceMode2D.Force);
			_body.drag = Mathf.Lerp(_flyDrag, 1f, _body.velocity.magnitude / _flyMaxVelocity);
			return false;
		}

		public override bool WennLaufen(Vector2 richtung) {
			var dirLen = richtung.magnitude;
			if (dirLen > 0.01f) {
				_magnitude = dirLen;
				_direction = richtung / dirLen;
			} else {
				_magnitude = 0f;
			}

			return false;
		}

		public override bool WennSpringen() {
			var bullet = _bulletPool.Get();
			bullet.transform.position                   = _shootPosition.position;
			bullet.GetComponent<Rigidbody2D>().velocity = _direction * _bulletVelocity;
			_body.AddForce(_recoilImpulse * -_direction, ForceMode2D.Impulse);
			return false;
		}

		private void Update() {
			// required to have enable
		}
	}
}
