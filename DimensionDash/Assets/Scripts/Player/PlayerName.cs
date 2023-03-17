using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Player {
	public class PlayerName : MonoBehaviour {
		[SerializeField] private string _name;

		[SerializeField] private TMP_Text _nameText;

		public string GetName() {
			return _name;
		}

		public void SetName(string name) {
			_name          = name;
			_nameText.text = name;
		}

		public void SetColor(Color color) {
			_nameText.color = color;
		}
	}
}
