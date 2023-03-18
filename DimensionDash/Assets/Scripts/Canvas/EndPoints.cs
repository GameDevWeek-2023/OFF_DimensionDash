using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using TMPro;
using UnityEngine;

namespace Canvas {
	[Serializable]
	public class Highscore_row {
		public TMP_Text name;
		public TMP_Text score;
	}
	
	public class EndPoints : MonoBehaviour {
		[SerializeField]         private float  _delay;

		[SerializeField] private Transform[] _positions;
		
		[SerializeField] private TMP_Text      _winnerText;
		[SerializeField] private Highscore_row[] _highscore_rows;

		[SerializeField] private TMP_Text _timerText;

		private float _delayLeft;

		void Start() {
			_delayLeft = _delay;

			var scores = new List<(int, string, Color, GameObject)>();

			foreach (var r in _highscore_rows) {
				r.name.gameObject.SetActive(false);
				r.score.gameObject.SetActive(false);
			}

			foreach (var p in PlayerManager.Instance.Players) {
				p.transform.parent.GetComponent<PlayerController>().Ready = false;

				var color = p.GetComponent<PlayerColor>();
				var name  = p.GetComponent<PlayerName>();
				var score = p.GetComponent<PlayerPoints>();

				if (color && name && score) {
					scores.Add((score.points, name.GetName(), color.GetColor().Color, p));
				}
			}

			if (scores.Count == 0)
				return;

			scores.Sort((lhs, rhs) => rhs.Item1-lhs.Item1);

			for (int i = 0; i < Math.Min(_highscore_rows.Length, scores.Count); ++i) {
				var (score, name, color, go) = scores[i];
				var row = _highscore_rows[i];
				
				row.name.gameObject.SetActive(true);
				row.name.text  = name;
				row.name.color = color;
				
				row.score.gameObject.SetActive(true);
				row.score.text  = score.ToString();
				row.score.color = color;

				go.transform.position = _positions[Math.Min(i, _positions.Length - 1)].position;
			}

			_winnerText.color = scores[0].Item3;
		}

		private void Update() {
			if (_delayLeft <= 0)
				return;

			if (!PlayerManager.Instance.Players.Any(p => p.transform.parent.GetComponent<PlayerController>().Ready)) {
				if(_timerText)
					_timerText.gameObject.SetActive(false);
				return;
			}

			_delayLeft -= Time.unscaledDeltaTime;
			if (_timerText) {
				_timerText.gameObject.SetActive(true);
				_timerText.text = Mathf.RoundToInt(_delayLeft).ToString();
			}

			if (_delayLeft <= 0) {
				GameStateManager.Instance.ChangeToInitialScreen();
			}
		}
		
	}
}
