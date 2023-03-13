using UnityEngine;

// Sehr einfaches Script zum Bewegen von Objekten, ohne Beschleunigung oder andere komplexe Berechnungen.
// Dieses Skript ist damit ein guter Startpunkt für komplett eigene ungewöhnlichere Steuerungen.
// Animationen und weitere Effekte passieren im Juice.cs Script
public class BewegenEinfach : BewegenBasis {
	[SerializeField] private float geschwindigkeit = 6f;
	[SerializeField] private float sprungkraft     = 300f;

	private void Update()
	{
		// Sprite in die Bewegungsrichtung ausrichten
		if(Richtung() != 0) {
			transform.localScale = new Vector3(Richtung() > 0 ? 1 : -1, 1, 1);
		}
	}

	protected override void Aktuallisieren()
	{
		if(Betäubt()) return;

		// Geschwindigkeit auf der X-Achse entsprechend der Eingabe (richtung) setzen
		// Es gibt hier also keine Beschleunigung oder ähnliches
		körper.velocity = new Vector2(geschwindigkeit * Richtung(), körper.velocity.y);

		if(springen) {
			springen = false;

			// Springen-Befehl ignorieren, wenn wir aktuell nicht auf dem Boden sind
			// Könnte man entfernen, wenn man auch in der Luft springen können soll
			if(!bodenkontakt.StehtAufDemBoden()) return;

			// Geschwindigkeit auf der Y-Achse auf 0 setzen, falls wir gerade fallen oder bereits springen
			körper.velocity = new Vector2(körper.velocity.x, 0f);

			// Spieler nach oben stoßen
			körper.AddForce(Vector2.up * sprungkraft, ForceMode2D.Impulse);

			// Sprung-Effekte abspielen (falls vorhanden)
			if(juice) juice.SprungEffekte();
		}
	}
}
