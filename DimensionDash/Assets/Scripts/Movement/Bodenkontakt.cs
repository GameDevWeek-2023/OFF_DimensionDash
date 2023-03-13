using UnityEngine;

// Wird benutzt um zu überprüfen ob der Spieler aktuell auf dem Boden steht, z.B. für's Springen
[ExecuteInEditMode]
public class Bodenkontakt : MonoBehaviour {
	public float kontaktLänge = 0.95f;

	[Tooltip("Mittelpunkt der Kontaktfläche, relativ zum Spieler")]
	public Vector2 kontaktPosition = new Vector2(0, 0);

	[Tooltip("Maximaler Abstand zwischen dem Boden und dem Kontakt, der gerade noch als 'auf dem Boden stehen' gewertet wird")]
	public float maxHöhe = 0.5f;

	[Tooltip("Erlaubt es einzustellen welche Objekte als 'Boden' gewertet werden")]
	public LayerMask bodenLayer = ~0; // ~0 heißt hier *alles*

	private bool bodenLinks = false;
	private bool bodenRechts = false;

	// Methoden um abzufragen ob wir auf dem Boden stehen
	public bool StehtAufDemBoden()      { return bodenLinks || bodenRechts; }
	public bool StehtAnEinerKante()     { return bodenLinks ^ bodenRechts; }
	public bool StehtLinksAufDemBoden() { return bodenLinks;}
	public bool StehtRechtsAufDemBoden() { return bodenRechts;}

	private void Update()
	{
		// Prüft ob es unter dem linken oder dem rechten Ende der Kontaktfläche Boden gibt oder nicht
		var scale     = transform.localScale;
		var p         = transform.position + new Vector3(kontaktPosition.x * scale.x, kontaktPosition.y * scale.y, 0f);
		var aPosition = p + Vector3.left * (scale.x * kontaktLänge / 2);
		var bPosition = p + Vector3.right * (scale.x * kontaktLänge / 2);

		var a = Physics2D.Raycast(aPosition, Vector2.down, maxHöhe, bodenLayer);
		var b = Physics2D.Raycast(bPosition, Vector2.down, maxHöhe, bodenLayer);

		bodenLinks  = scale.x > 0 ? a : b;
		bodenRechts = scale.x > 0 ? b : a;
	}

	// Zeichnet eine Visualisierung der Bodenfläche im Editor, damit es einfach ist die Werte passend einzustellen
	private void OnDrawGizmos()
	{
		if(!StehtAufDemBoden())
			Gizmos.color = Color.red;
		else if(StehtAnEinerKante())
			Gizmos.color = Color.yellow;
		else
			Gizmos.color = Color.green;

		var scale     = transform.localScale;
		var p         = transform.position + new Vector3(kontaktPosition.x * scale.x, kontaktPosition.y * scale.y, 0f);
		var aPosition = p + scale.x * Vector3.left * kontaktLänge / 2;
		var bPosition = p + scale.x * Vector3.right * kontaktLänge / 2;

		Gizmos.DrawLine(aPosition, bPosition);

		if(Physics2D.Raycast(aPosition, Vector2.down, maxHöhe, bodenLayer)) {
			Gizmos.color = Color.green;
		} else {
			Gizmos.color = Color.red;
		}
		Gizmos.DrawLine(aPosition, aPosition + Vector3.down * maxHöhe);

		if(Physics2D.Raycast(bPosition, Vector2.down, maxHöhe, bodenLayer)) {
			Gizmos.color = Color.green;
		} else {
			Gizmos.color = Color.red;
		}
		Gizmos.DrawLine(bPosition, bPosition + Vector3.down * maxHöhe);
	}
}
