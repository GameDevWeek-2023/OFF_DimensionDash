using UnityEngine;

namespace Movement {
	// Kann an ein GameObject mit einem Trigger-Collider angehängt werden und aktiviert die Kletter-Steuerung bei
	//   allen Objekten die in den Trigger kommen und eine entsprechende 'KletternBewegung' Komponente haben.
	public class Klettern : MonoBehaviour {
		private void OnTriggerEnter2D(Collider2D kollision) {
			if(kollision.TryGetComponent(out KletternBewegung bewegen)) {
				bewegen.kletterbareObjekte++;
				bewegen.enabled = true;
			}
		}

		private void OnTriggerExit2D(Collider2D kollision) {
			if(kollision.TryGetComponent(out KletternBewegung bewegen)) {
				bewegen.kletterbareObjekte--;
				if(bewegen.kletterbareObjekte==0)
					bewegen.enabled = false;
			}
		}
	}
}
