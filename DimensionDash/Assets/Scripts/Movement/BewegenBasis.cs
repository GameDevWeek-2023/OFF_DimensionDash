using System.Collections;
using UnityEngine;

// Erlaubt das schreiben von Komponentnen um die Bewegungssteuerung temporär zu ändern, z.B. für Klettern, Fliegen o.ä.
// Die Funktionen unten werden von den eigentlichen Bewegen-Skripten aufgerufen und können 'false' zurückgeben um die 
//   eigentliche Bewegung zu überspringen und statt dessen eigenen Code auszuführen.
// Siehe: KletternBewegung.cs und Klettern.cs für ein Beispiel 
public abstract class BewegenÜberschreiben : MonoBehaviour {
	public virtual bool WennSpringen()               { return true; }
	public virtual bool WennLaufen(Vector2 richtung) { return true; }
	public virtual bool WennAktuallisieren()         { return true; }
	public virtual bool WennBetäuben()               { return true; }
}

// Dient als Basis für alle Objekte die sich selbstständig bewegen können bzw. durch den Spieler gesteuert werden.
// Also sowohl der Spieler als auch Gegner.
// Hier finden sich einige allgemeine Eigenschaften und Operationen.
// Von dem eigentlichen Code für die Bewegung gibt es zwei Varianten:
//   - BewegenEinfach.cs        : Eine sehr einfache Steuerung, ohne Beschleunigung oder komplexe Sprung-Physik, die
//                                dafür aber einfach zu verstehen, anzupassen und zu erweitern ist.
//   - BewegenPlatformerToolkit : Eine komplexere Steuerung, mit Beschleunigung, Sprung-Physik und umfangreichen
//                                Einstellungsmöglichkeiten.
//                                Basiert auf dem "Platformer Toolkit" von Mark Brown.
//                                Siehe: https://gmtk.itch.io/platformer-toolkit
[RequireComponent(typeof(Rigidbody2D), typeof(Bodenkontakt)), DisallowMultipleComponent,SelectionBase]
public abstract class BewegenBasis : MonoBehaviour {
	// Verweise auf andere Komponenten des GameObjects, die wir für die Bewegung brauchen
	protected Rigidbody2D  körper;       // u.a. um die Geschwindigkeit zu ändern
	protected Bodenkontakt bodenkontakt; // um zu prüfen ob wir auf dem Boden stehen oder nicht
	protected Juice        juice;        // für Animation und andere Effekte

	// Variablen in denen wir uns die letzte Eingabe merken
	protected float richtung          = 0;     // ob wir nach links (< 0), nach rechts (> 0) oder gar nicht laufen sollen
	protected bool  springen          = false; // ob wir einen Sprung ausführen sollen
	protected bool  springenAbbrechen = false; // ob die Springen-Taste losgelassen wurde und wir den Sprung ggf. abbrechen sollen

	// Variablen mit denen die "Betäubung" von Gegnern/Spielern gesteuert werden.
	// Passiert u.a. wenn der Spieler Schaden nimmt, z.B. weil er einen Gegner berührt hat, und blockiert die Eingabe für kurze Zeit
	private bool      betäubt = false;
	private Coroutine betäubenCoroutine;

	private BewegenÜberschreiben[] überschreibungen;

	// Wann das Objekt erzeugt wird lokalisieren wir die benötigten Komponenten und merken sie uns in den Variablen oben
	private void Start()
	{
		körper           = GetComponent<Rigidbody2D>();
		bodenkontakt     = GetComponent<Bodenkontakt>();
		juice            = GetComponent<Juice>();
		überschreibungen = GetComponents<BewegenÜberschreiben>();
	}

	public bool  Betäubt()  { return betäubt; }
	public float Richtung() { return richtung; }

	// Methoden um die Bewegung zu steuern.
	// Werden von Gegner/GegnerSteuerung.cs und Spieler/SpielerSteuerung.cs aufgerufen
	public void SpringenStarten()
	{
		if(betäubt) return;

		if (überschreibungen != null) {
			foreach (var x in überschreibungen) {
				if (x.enabled && !x.WennSpringen())
					return;
			}
		}

		springen          = true;
		springenAbbrechen = false;
	}
	public void SpringenAbbrechen() { springenAbbrechen = true; }
	public void Laufen(Vector2 wunschRichtung)
	{
		if(betäubt) return;

		richtung = 0;

		if (überschreibungen != null) {
			foreach (var x in überschreibungen) {
				if (x.enabled && !x.WennLaufen(wunschRichtung))
					return;
			}
		}

		richtung = Mathf.Abs(wunschRichtung.x)<=1f ? wunschRichtung.x : Mathf.Sign(wunschRichtung.x);
	}

	private void FixedUpdate() {
		if (überschreibungen != null) {
			foreach (var x in überschreibungen) {
				if (x.enabled && !x.WennAktuallisieren())
					return;
			}
		}

		Aktuallisieren();
	}

	protected abstract void Aktuallisieren();

	// Methode um den Spieler/Gegner für X Sekunden zu betäuben
	public void Betäuben(float sekunden)
	{
		if (überschreibungen != null) {
			foreach (var x in überschreibungen) {
				if (x.enabled && !x.WennBetäuben())
					return;
			}
		}

		betäubt  = true;
		richtung = 0;
		if(juice) juice.SchadensEffekteStarten();

		// Code um die Betäubung nach der Wartezeit wieder aufzuheben
		if(betäubenCoroutine != null) StopCoroutine(betäubenCoroutine);
		betäubenCoroutine = StartCoroutine(BetäubenAufheben(sekunden));
	}
	private IEnumerator BetäubenAufheben(float sekunden)
	{
		yield return new WaitForSeconds(sekunden);
		if(juice) juice.SchadensEffekteBeenden();
		betäubt = false;
	}
}
