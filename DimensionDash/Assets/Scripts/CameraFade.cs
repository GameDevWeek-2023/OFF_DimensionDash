using UnityEngine;

public class CameraFade : MonoBehaviour {
	[SerializeField]
	[Tooltip("How fast should the texture be faded out?")]
	[Range(0, 10)]
	private float _fadeTime = 5.0f;

	[SerializeField]
	[Tooltip("Choose the color, which will fill the screen.")]
	private Color _fadeColor = new(255.0f, 255.0f, 255.0f, 1.0f);

	[SerializeField]
	[Tooltip("How long will the screen stay black during FadeIn?")]
	[Range(0, 10)]
	private float _blackScreenDuration;

	private float     _alpha = 1.0f;
	private Texture2D _texture;

	private bool _isFadingIn  = false;
	private bool _isFadingOut = false;

	private float _currentTime = 0;
	private bool  _firstFrame  = true;

	public class Options {
		public float? FadeTime;
		public float? BlackScreenDuration;
		public Color? FadeColor;
	}

	private void Start() {
		_texture = new Texture2D(1, 1);
		_texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
		_texture.Apply();
		FadeIn();
	}

	private void StartFading(bool isFadingIn, bool isFadingOut, Options options = null) {
		_currentTime = 0;

		if (options != null) {
			if (options.FadeTime != null) _fadeTime = (float) options.FadeTime;

			if (options.BlackScreenDuration != null) _blackScreenDuration = (float) options.BlackScreenDuration;

			if (options.FadeColor.HasValue) _fadeColor = options.FadeColor.Value;
		}

		_isFadingIn  = isFadingIn;
		_isFadingOut = isFadingOut;
	}

	public void FadeIn(Options options = null) {
		_alpha = 1.0f;
		StartFading(true, false, options);
	}

	public void FadeOut(Options options = null) {
		_alpha = 0.0f;
		StartFading(false, true, options);
	}

	public void OnGUI() {
		if (_isFadingIn || _isFadingOut) ShowBlackScreen();
	}

	private void ShowBlackScreen() {
		if (_isFadingIn && _alpha <= 0.0f) {
			_isFadingIn = false;

			return;
		}

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);

		if (_isFadingIn && _blackScreenDuration > 0) {
			_blackScreenDuration -= Time.unscaledDeltaTime;
			return;
		}

		if (_isFadingOut && _alpha >= 1.0f) return;

		CalculateTexture();
	}

	private void CalculateTexture() {
		if (_firstFrame) {
			_firstFrame = false;
			return;
		}

		_currentTime += Time.unscaledDeltaTime;

		if (_isFadingIn) _alpha = 1.0f - (_currentTime / _fadeTime);
		else _alpha             = _currentTime / _fadeTime;

		_texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
		_texture.Apply();
	}
}