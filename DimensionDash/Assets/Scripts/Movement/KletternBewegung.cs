using UnityEngine;

namespace Skripte.Bewegung {
	// Überschreibt die normale Steuerung (aktiviert durch Klettern.cs und drücken von 'w' bzw. Pfeil-Hoch) mit der
	//   Steuerung für Klettern, in der der Spieler nicht mehr nach unten fällt und sich in beliebige Richtungen bewegen kann.
	// Die Komponente ist normalerweise deaktiviert und wird nur aktiviert wenn der Spieler vor einem
	//   Objekt steht an dem er hochklettern kann.
	class KletternBewegung : BewegenÜberschreiben {
		public float kletterGeschwindigkeit = 2f;

		private Rigidbody2D  körper;       // u.a. um die Geschwindigkeit zu ändern
		private Bodenkontakt bodenkontakt; // um zu prüfen ob wir auf dem Boden stehen oder nicht
		private Animator     animator;     // um Animationen abzuspielen

		// Steuert ob gerade tatsächlich geklettert oder die normale Steuerung verwendet wird.
		// Wird auch 'true' gesetzt wenn der Spieler versucht sich nach oben zu bewegen ('W') oder fällt und 'S' drückt.
		// Wird wieder auf 'false' gesetzt wenn der Spieler auf den Boden zurückgehrt, Springt oder Schaden nimmt.
		private bool    klettert          = false;
		private bool    warSchonInDerLuft = false; // Gibt an ob der Spieler seitdem er Klettert schon in der Luft war
		private Vector2 kletterRichtung;

		// Zählt vor wie vielen Objekten an den wir hochklättern können wir gerade stehen.
		public int kletterbareObjekte = 0;

		// Wann das Objekt erzeugt wird lokalisieren wir die benötigten Komponenten und merken sie uns in den Variablen oben
		private void Start() {
			körper       = GetComponentInChildren<Rigidbody2D>();
			bodenkontakt = GetComponentInChildren<Bodenkontakt>();
			animator     = GetComponentInChildren<Animator>();
		}

		private void OnDisable() { NichtKlettern(); }

		public override bool WennAktuallisieren() {
			// Wird in jedem Frame aufgerufen
			if(klettert) {
				// Wenn wir gerade klettern, prüfen wir ob wir schon wieder auf dem Boden sind
				if(!bodenkontakt.StehtAufDemBoden()) {
					warSchonInDerLuft = true;
				} else if(warSchonInDerLuft) {
					NichtKlettern();
					return true;
				}

				// ... sonst Ändern wir die Animation und Setzen die Geschwindigkeit entsprechend der Eingabe
				animator.SetBool("klettern", true);
				körper.velocity = kletterRichtung * kletterGeschwindigkeit;
				return false; // (normale Bewegungs-Logik wird nicht ausgeführt)
			}

			// Wird klettern nicht => Normale Logik vom Bewegen-Skript ausführen.
			return true;
		}

		// Wird ausgeführt wenn wir anfangen zu klettern und deaktiviert u.a. die Erdanziehungskraft.
		private void Klettern() {
			if(klettert) return;

			klettert            = true;
			körper.gravityScale = 0;
			//körper.isKinematic = true;
			warSchonInDerLuft   = !bodenkontakt.StehtAufDemBoden();
			animator.SetBool("klettern", true);
		}

		// Wird ausgeführt wenn wir aufhören zu klettern und macht das was wir in Klettern() gemacht haben rückgängig. 
		private void NichtKlettern() {
			if(!klettert) return;

			klettert            = false;
			körper.gravityScale = 1;
			//körper.isKinematic  = false;
			animator.SetBool("klettern", false);
		}

		public override bool WennLaufen(Vector2 richtung) {
			// Wir fangen an zu klettern, wenn der Spieler "nach-oben" ('W') drückt,
			//   oder wenn er "nach-unten" ('S') drückt und vorher nicht schon nach unten geklettert ist.
			if((warSchonInDerLuft ? richtung.y : Mathf.Abs(richtung.y)) > 0.1) {
				Klettern();
			}

			if(richtung.y >= 0) warSchonInDerLuft = false;

			kletterRichtung = richtung;

			return true;
		}

		public override bool WennSpringen() {
			// Wenn der Spieler springt hören wir immer auf zu klettern
			warSchonInDerLuft = false;
			NichtKlettern();
			return true;
		}

		public override bool WennBetäuben() {
			// Wenn der Spieler betäubt wird (meistens weil ihn ein Gegner getroffen hat) hören wir auch auf zu klettern
			warSchonInDerLuft = false;
			NichtKlettern();
			return true;
		}
	}
}
