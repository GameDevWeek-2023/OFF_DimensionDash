using JetBrains.Annotations;
using UnityEngine;

public class GlobalSystem<T> : MonoBehaviour where T : GlobalSystem<T> {
	private static T _instance;

	[CanBeNull] public static T InstanceOptional => _instance;

	public static T Instance {
		get {
			if (!_instance)
				Debug.LogError($"No Instance of {typeof(T).Name}");
			return _instance;
		}
	}

	private void Register() {
		if (_instance == this)
			return;
		else if (_instance)
			Debug.LogError($"Multiple instances of {typeof(T).Name}");
		else
			_instance = (T) this;
	}

	protected virtual void Awake() {
		if (isActiveAndEnabled) Register();
	}

	protected virtual void OnEnable() {
		Register();
	}

	protected virtual void OnDestroy() {
		if (_instance == this)
			_instance = null;
		else
			Debug.LogError($"Multiple instances of {typeof(T).Name}");
	}
}
