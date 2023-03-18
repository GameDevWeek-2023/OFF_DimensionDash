using DG.Tweening;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Movement {
	public class BewegenFlipper : BewegenÜberschreiben {
		[SerializeField] private float _speed       = 20f;
		[SerializeField] private float _pushImpulse = 500f;
		[SerializeField] private float _maxVelocity  = 60f;

		[SerializeField] private Rigidbody2D   _armBody;
		[SerializeField] private BoxCollider2D _armCollider;
		[SerializeField] private Rigidbody2D   _playerBody;
		[SerializeField] private BewegenBasis  _basis;

		private Vector2        _direction;
		private Vector3        _lastCameraPosition;
		private float          _lastPush;
		private SpriteRenderer[] _armSprites;

		private void OnValidate() {
			_playerBody = GetComponent<Rigidbody2D>();
			_basis      = GetComponent<BewegenPlatformToolkit>();
		}

		public override bool WennAktuallisieren() {
			if (_playerBody.velocity.sqrMagnitude > _maxVelocity * _maxVelocity)
				_playerBody.velocity = _maxVelocity * _playerBody.velocity.normalized;
			return false;
		}

		private void FixedUpdate() {
			if (_direction.magnitude < 0.01f)
				return;

			var p    = _armBody.position + _direction * (_speed * Time.deltaTime);
			var camP = Camera.main.transform.position;
			p                   += new Vector2(camP.x - _lastCameraPosition.x, camP.y - _lastCameraPosition.y);
			_lastCameraPosition =  camP;
			_armBody.MovePosition(LimitPosition(p));
		}

		private void Start() {
			_armBody.GetComponent<PlayerColor>().SetColor(_playerBody.GetComponent<PlayerColor>().GetColor(), () => { });
			_armSprites = _armBody.GetComponentsInChildren<SpriteRenderer>();
		}

		public void EnableMode() {
			_basis.Laufen(Vector2.zero);
			_armBody.gameObject.SetActive(true);
			_armBody.position   = LimitPosition(_playerBody.position - Vector2.up * 4f);
			_lastCameraPosition = Camera.main.transform.position;
			_playerBody.AddForce(Vector2.up * 500 + Vector2.right * Random.Range(-200, 200), ForceMode2D.Impulse);
		}

		private void OnEnable() {
			if (_armSprites != null) {
				foreach (var s in _armSprites) {
					var c = s.color;
					c.a     = 1f;
					s.color = c;
				}
			}
		}

		private void OnDisable() {
			if (_armSprites != null) {
				foreach (var s in _armSprites) {
					var c = s.color;
					c.a     = 0.25f;
					s.color = c;
				}
			}
		}

		public void DisableMode() {
			_armBody.gameObject.SetActive(false);
		}

		private Vector3 LimitPosition(Vector3 p) {
			var cam = Camera.main;
			if (!cam)
				return p;

			var camPosition = cam.transform.position;
			var camSize     = new Vector2(cam.aspect, 1f) * cam.orthographicSize;

			p.y = Mathf.Clamp(p.y, camPosition.y - camSize.y + _armCollider.size.y, camPosition.y + camSize.y - _armCollider.size.y);
			p.x = Mathf.Clamp(p.x, camPosition.x - camSize.x + _armCollider.size.x, camPosition.x + camSize.x);

			return p;
		}

		public override bool WennLaufen(Vector2 richtung) {
			_direction = richtung;
			return false;
		}

		private void OnCollisionEnter2D(Collision2D col) {
			if (col.rigidbody == _playerBody && (Time.time - _lastPush) <= 0.25f && col.contacts.Length > 0) {
				col.rigidbody.velocity = Vector2.zero;
				col.rigidbody.AddForce(col.GetContact(0).normal * _pushImpulse);
			}
		}

		public override bool WennSpringen() {
			_lastPush = Time.time;

			var sequence = DOTween.Sequence();
			sequence.Append(_armBody.DORotate(-45f, 0.2f).SetEase(Ease.OutElastic));

			sequence.Append(_armBody.DORotate(0, 0.1f).SetEase(Ease.InOutExpo));
			return false;
		}

		private void Update() {
			// required to have enable
		}
	}
}
