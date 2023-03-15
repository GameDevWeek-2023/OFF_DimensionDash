using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Canvas {
	public class OutOfViewMarkerPool : GlobalSystem<OutOfViewMarkerPool> {

		[SerializeField] private GameObject _prefab;
		
		private ObjectPool<GameObject> _pool;

		protected override void Awake() {
			base.Awake();
			_pool = new ObjectPool<GameObject>(() => Instantiate(_prefab, transform), (obj) => obj.SetActive(true), obj=>obj.SetActive(false), Destroy,
			                                false, 8, 16);
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			_pool.Dispose();
			_pool = null;
		}

		public Image GetMarker() {
			return _pool.Get().GetComponent<Image>();
		}

		public void ReturnMarker(Image marker) {
			if(_pool!=null)
				_pool.Release(marker.gameObject);
		}
		
	}
}
