using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Canvas {
	public class OutOfViewMarkerPool : GlobalSystem<OutOfViewMarkerPool> {

		[SerializeField] private GameObject _prefab;
		
		private ObjectPool<GameObject> _pool;

		private void Awake() {
			_pool = new ObjectPool<GameObject>(() => Instantiate(_prefab, transform), (obj) => obj.SetActive(true), obj=>obj.SetActive(false), Destroy,
			                                false, 8, 16);
		}

		private void OnDestroy() {
			_pool.Dispose();
		}

		public Image GetMarker() {
			return _pool.Get().GetComponent<Image>();
		}

		public void ReturnMarker(Image marker) {
			_pool.Release(marker.gameObject);
		}
		
	}
}
