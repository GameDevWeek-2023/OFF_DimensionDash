using NaughtyAttributes;
using UnityEngine;

// Das hier ist ein etwas umfangreiches Skript zur Bewegung von Objekten, basierend auf dem  "Platformer Toolkit" von Mark Brown.
// Die Optionen sind genauso benannt wie dort, sodass Einstellung direkt übernommen werden können.
// Zu den Funktionen die hier verfügbar sind zählen u.a.:
//   - Beschleunigung bei Bewegungen, die detailliert konfiguriert werden kann
//   - Detailliert konfigurierbare Sprünge
//   - Steuern der Sprunghöhe dadurch wie lange die Taste gedrückt gehalten wird
//   - Mehrere Sprünge in der Luft
//   - Coyote-Time (Springen obwohl der Boden bereits verlassen wurde)
//   - Jump-Buffer (Die Sprungtaste kann bereits gedrückt werden obwohl der Spieler noch nicht wieder zurück auf dem Boden ist)
public class BewegenPlatformToolkit : BewegenBasis {
	// Optionen/Einstellungen zur horizontalen Bewegung
	[Header("Running (Laufen)"), Range(0f, 20f), Tooltip("Maximale Bewegungsgeschwindigkeit (in m/s)")]
	public float maxSpeed = 10f;

	[Range(0f, 100f), Tooltip("Maximale Beschleunigung (in m/2²). Also wie schnell wir die max. Geschwindigkeit erreichen"), EnableIf(nameof(useAcceleration))]
	public float maxAcceleration = 52f;

	[Range(0f, 100f), Tooltip("Maximale Beschleunigung (in m/2²) beim Anhalten. Also wie schnell wir stehen bleiben wenn keine Taste gedrückt wird"), EnableIf(nameof(useAcceleration))]
	public float maxDeceleration = 52f;

	[Range(0f, 100f),
	 Tooltip("Maximale Beschleunigung (in m/2²) bei Richtungsänderungen. Also wie schnell wir zwischen 'nach links laufen' und 'nach rechts laufen' wechseln können"), EnableIf(nameof(useAcceleration))]
	public float maxTurnSpeed = 80f;

	[Range(0f, 100f), Tooltip("Maximale Beschleunigung (in m/2²) wenn wir in der Luft sind (fallen/springen)"), EnableIf(nameof(useAcceleration))]
	public float maxAirAcceleration = 20f;

	[Range(0f, 100f), Tooltip("Maximale Beschleunigung (in m/2²) beim Anhalten in der Luft (fallen/springen)"), EnableIf(nameof(useAcceleration))]
	public float maxAirDeceleration = 20f;

	[Range(0f, 100f), Tooltip("Maximale Beschleunigung (in m/2²) bei Richtungsänderungen in der Luft (fallen/springen)"), EnableIf(nameof(useAcceleration))]
	public float maxAirTurnSpeed = 80f;

	[Tooltip("Aktiviert/Deaktiviert die Beschleunigung die oben definiert wird. Wenn das hier deaktiviert ist, verhält sich die Steuerung ähnlich wie die aus BewegenEinfach.cs")]
	public bool useAcceleration = true;

	[Range(0f, 50f), Tooltip("Die Bewegungsgeschwindigkeit (in m/s), die der Spieler bekommt wenn er von einer Wand abspringt")]
	public float wallJumpSpeed = 20f;


	// Optionen/Einstellungen zum Springen
	[Header("Jumping (Springen)"), Range(2f, 5.5f), Tooltip("Die Maximale Höhe in Metern, die der Spieler springen kann")]
	public float jumpHeight = 4.0f;

	[Range(0.5f, 10f), Tooltip("Die Zeit in Sekunden zwischen dem Absprung und der Landung")]
	public float jumpDuration = 2f;

	[Range(0f, 5f), Tooltip("Faktor um die Gravitation während dem Springen zu erhöhen (>1) oder zu verringern (<1)")]
	public float upwardMovementMultiplier = 1f;

	[Range(0f, 10f), Tooltip("Faktor um die Gravitation beim Fallen zu erhöhen (>1) oder zu verringern (<1)")]
	public float downwardMovementMultiplier = 1f;

	[Range(-1, 10), Tooltip("Anzahl der Sprünge die wir in der Luft machen können (-1 = beliebig viele). 'Double Jump' im Toolkit entspricht hier 1")]
	[SerializeField]private int maxAirJumps = 1;

	[Tooltip("Soll die Sprunghöhe davon abhängen wie lange die Taste gedrückt wird? Wenn das aktiviert ist beginnt der Spieler zu fallen sobald die Taste losgelassen wurde.")]
	public bool variableJumpHeight = true;

	[Range(1f, 10f), EnableIf(nameof(variableJumpHeight)),
	 Tooltip("Steuert wie stark der Spieler nach unten gezogen wird wenn die Sprungtaste losgelassen wurde und 'variableJumpHeight' aktiviert ist")]
	public float jumpCutOff = 3;

	[Header("Assists"), Range(0f, 0.3f),
	 Tooltip("Dauer der Coyote-Time in Sekunden. Also wie viel Zeit zwischen dem verlassen des Bodens und einem Sprung liegen darf")]
	public float coyoteTime = 0.15f;

	[Range(0f, 0.3f),
	 Tooltip("Dauer des Jump-Buffers in Sekunden. Also wie viel 'zu früh' wir die Sprungtaste drücken dürfen, ohne das die Eingabe ignoriert wird")]
	public float jumpBuffer = 0.15f;

	[Range(0, 50), Tooltip("Die schnellste Geschwindigkeit mit der wir fallen können (in m/s)")]
	public float terminalVelocity = 20;


	// Zustände die wir uns dafür merken müssen
	private int   verbleibendeAirJumps;
	private float sekundenSeitBodenkontakt; // für Coyote-Time
	private bool  springtGerade;
	private float sekundenSeitSprungBefehl; // für Jump-Buffer

	public int MaxAirJumps {
		get => maxAirJumps;
		set {
			if (maxAirJumps < 0 && value >= 0) verbleibendeAirJumps = value;
			if (value < verbleibendeAirJumps) verbleibendeAirJumps  = value;

			maxAirJumps = value;
		}
	}

	protected override void Aktuallisieren()
	{
		GravitationFestlegen();

		if(richtung != 0) {
			// Wenn wir uns gerade bewegen, prüfen wir die Richtung und spiegeln ggf. das Objekt entlang der X-Achse
			var skalierung = transform.localScale;
			skalierung.x         = (richtung > 0 ? 1 : -1) * Mathf.Abs(skalierung.x);
			transform.localScale = skalierung;
		}

		if(bodenkontakt.StehtAufDemBoden() && Mathf.Abs(körper.velocity.y) < 0.01f) {
			// Wenn wir auf dem Boden stehen und uns nicht nach oben/unten bewegen (nicht gerade abspringen),
			//   müssen wir die Variablen für die Air-Jumps und Coyote-Time zurücksetzen
			sekundenSeitBodenkontakt = 0;
			springtGerade            = false;
			verbleibendeAirJumps     = maxAirJumps;
		}

		if(!bodenkontakt.StehtAufDemBoden() && !springtGerade) {
			// Wenn wir in der Luft sind ohne gesprungen zu sein, sind wir über das Ende einer Plattform gelaufen
			//   und müssen den Timer für die Coyote-Time hochzählen
			sekundenSeitBodenkontakt += Time.deltaTime;
		}

		LaufenDurchführen();
		if(springen) {
			SpringenDurchführen();
		}

		// Wir beschränken die Y-Geschwindigkeit, sodass wir uns nach unten maximal mit der in 'terminalVelocity' festgelegten Geschwindigkeit bewegen können
		körper.velocity = new Vector2(körper.velocity.x, Mathf.Max(körper.velocity.y, -terminalVelocity));
	}

	private void GravitationFestlegen()
	{
		// Berechnet die Gravitation des Spielers aus der gewünschten Sprunghöhe, Dauer und dem Gravitationsfaktor
		var t          = (jumpDuration - 1f) * (2.5f - 0.2f) / (10f - 1f) + 0.2f;
		var newGravity = new Vector2(0, -2 * jumpHeight / (t * t));
		körper.gravityScale = newGravity.y / Physics2D.gravity.y * GravitationsfaktorBerechnen();
	}
	private float GravitationsfaktorBerechnen()
	{
		if(bodenkontakt.StehtAufDemBoden()) return 1f; // Die Faktoren haben nur Auswirkungen wenn wir springen oder fallen

		if(körper.velocity.y > 0.01f) { // Der Faktor für's Springen unterscheidet sich...
			if(variableJumpHeight && springtGerade && springenAbbrechen) {
				return jumpCutOff; // ... falls wir die Variable-Sprunghöhe verwenden und die Taste losgelassen wurde
			}
			return upwardMovementMultiplier;
		} else {
			return downwardMovementMultiplier; // Der Faktor für's Fallen ist immer der selbe
		}
	}

	private void LaufenDurchführen()
	{
		// Wir berechnen uns die Geschwindigkeit die wir erreichen wollen, aus der eingegebenen Richtung und der maximalen Geschwindigkeit
		var zielGeschwindigkeit = richtung * Mathf.Max(maxSpeed, 0f);

		var geschwindigkeit = körper.velocity;
		if(useAcceleration) {
			// Wenn wir Beschleunigung verwenden, müssen wir zuerst prüfen welcher Fall vorliegt (Beschleunigung()-Methode unten) und anschließend
			//   unsere aktuelle Geschwindigkeit zu unserer Zielgeschwindigkeit bewegen, wobei wir die Geschwindigkeit pro Frame maximale
			//   um Beschleunigung*VergangeneZeit viel ändern
			geschwindigkeit.x = Mathf.MoveTowards(geschwindigkeit.x, zielGeschwindigkeit, Beschleunigung()*Time.fixedDeltaTime);
		} else {
			// Wenn wir keine Beschleunigung verwenden sollen setzen wir einfach die aktuelle auf die Ziel-Geschwindigkeit
			geschwindigkeit.x = zielGeschwindigkeit;
		}
		körper.velocity = geschwindigkeit;
	}
	private float Beschleunigung()
	{
		if(richtung == 0) {
			// Wir versuchen anzuhalten und geben entsprechend die passende 'Deceleration' zurück
			return bodenkontakt.StehtAufDemBoden() ? maxDeceleration : maxAirDeceleration;
		} else {
			// Wenn wir uns bewegen wollen prüfen wir das Vorzeichen um herauszufinden ob wir...
			if((int)Mathf.Sign(richtung) == (int)Mathf.Sign(körper.velocity.x)) {
				// ... weiterhin in die selbe Richtung laufen ...
				return bodenkontakt.StehtAufDemBoden() ? maxAcceleration : maxAirAcceleration;
			} else {
				// ... oder versuchen umzudrehen
				return bodenkontakt.StehtAufDemBoden() ? maxTurnSpeed : maxAirTurnSpeed;
			}
		}
	}

	private void SpringenDurchführen()
	{
		// Zuerst müssen wir prüfen ob wir überhaupt springen dürfen
		if(!SpringenErlaubt()) {
			// Wir sollten springen, dürfen das aber aktuell nicht
			if(sekundenSeitSprungBefehl < jumpBuffer) {
				// Wenn es einen Jump-Buffer gibt und wir noch Zeit übrig haben zählen wir nur die Zeit hoch.
				// Im nächsten Frame wird dann SprungDurchführen() nochmal aufgerufen, weil 'springen' immer noch 'true' ist und bis dahin
				//   sind wir eventuell schon wieder auf dem Boden und können dann springen.
				sekundenSeitSprungBefehl += Time.fixedDeltaTime;
			} else {
				// Wenn es keinen Jump-Buffer gibt oder die Zeit um ist geben wir auf
				springen                 = false;
				sekundenSeitSprungBefehl = 0;
			}

			return; // In jedem Fall können wir aber in diesem Frame (noch) nicht springen
		}

		var ersterSprung = !springtGerade;

		springen                 = false;        // Wir haben den Wunsch zu Springen erfüllt
		sekundenSeitBodenkontakt = coyoteTime+1; // Die Coyote-Time soll nicht greifen wenn wir bereits gesprungen sind
		sekundenSeitSprungBefehl = 0;
		springtGerade            = true;

		// Um zu springen setzen wir direkt die Y-Geschwindigkeit auf den passenden Wert für die gewünschte Sprung-Höhe
		var geschwindigkeit = körper.velocity;
		geschwindigkeit.y   = Mathf.Sqrt(-2f * Physics2D.gravity.y * körper.gravityScale * jumpHeight);
		if(!ersterSprung && bodenkontakt.StehtAnEinerKante() && verbleibendeAirJumps==0) {
			// Wir springen von einer Wand ab
			richtung          = bodenkontakt.StehtLinksAufDemBoden() ? 1 : -1;
			geschwindigkeit.x = richtung * wallJumpSpeed;
		}
		körper.velocity     = geschwindigkeit;

		if(juice != null) juice.SprungEffekte();
	}

	private bool SpringenErlaubt()
	{
		// Wenn wir auf dem Boden sind dürfen wir immer springen
		if(bodenkontakt.StehtAufDemBoden()) return true;

		// ... genauso wenn wir noch innerhalb der Coyote-Time sind
		if(sekundenSeitBodenkontakt < coyoteTime) return true;

		// ... oder wenn wir unbegrenzt viele Air-Jumps haben
		if(verbleibendeAirJumps < 0) return true;

		// Wenn wir noch Air-Jumps haben dürfen wir ebenfalls springen, müssen aber unsere Anzahl um eins runter zählen
		if(verbleibendeAirJumps > 0) {
			verbleibendeAirJumps--;
			return true;
		}

		// Andernfalls ist Springen aktuell nicht erlaubt
		return false;
	}
}
