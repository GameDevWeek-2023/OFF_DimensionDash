using System;
using System.Collections;
using UnityEngine;

namespace Movement {
	// Einstellung zum Verzerren des Sprites beim Springen und Landen (Squash & Stretch)
	[Serializable]
	public struct SquashSettings {
		[Tooltip("Faktor zum Verzerren in der Breite (<1 schmaler, >1 breiter)")]
		public float breiteFaktor;

		[Tooltip("Faktor zum Verzerren in der Höhe (<1 kleiner, >1 größer)")]
		public float höheFaktor;

		[Tooltip("Sekunden bis wir zur ursprünglichen Breite/Höhe zurückkehren")]
		public float dauerSekunden;

		public SquashSettings(float breiteFaktor, float höheFaktor, float dauerSekunden)
		{
			this.breiteFaktor  = breiteFaktor;
			this.höheFaktor    = höheFaktor;
			this.dauerSekunden = dauerSekunden;
		}
	}

// Kümmert sich um aesthetische Effekte wie Animationen, Partikel, Drehungen oder Squash & Stretch
	[RequireComponent(typeof(BewegenPlatformToolkit))]
	public class Juice : MonoBehaviour {
		[Header("Partikel-Effekte")]
		public ParticleSystem moveParticles;
		public ParticleSystem jumpParticles;
		public ParticleSystem landParticles;

		[Header("Sound-Effekte")]
		public AudioSource jumpSoundEffect;

		public AudioSource landSoundEffect;

		public SquashSettings jumpSquashSettings = new SquashSettings(0.5f, 1.8f, 0.1f);
		public SquashSettings landSquashSettings = new SquashSettings(1.5f, 0.4f, 0.2f);

		[Tooltip("Steuert wie stark der 'Squash & Stretch'-Effekt beim Landen ist (<1 : aus, =2 : wie oben definiert)"), Range(0, 2)]
		public float landSqueezeMultiplier = 1.5f;

		[Tooltip("Steuert wie stark der 'Squash & Stretch'-Effekt beim Springen ist (<1 : aus, =2 : wie oben definiert)"), Range(0, 2)]
		public float jumpSqueezeMultiplier = 1.5f;

		public bool squashAndStretch = true;
		
		[Tooltip("Steuert wie stark der Spieler beim 'Squash & Stretch'-Effekt vom Landen nach unten gezogen wird, damit er nicht über dem Boden schwebt")]
		public float landDrop = 0.4f;

		[Tooltip("Winkel in Grad um den das Sprite beim Laufen gedreht werden soll"), Range(-45, 45)]
		public float leanAngle = 10;

		[Tooltip("Wie schnell die Drehung passieren soll (in Grad/Sekunde)"), Range(0, 200)]
		public float leanSpeed = 80;

		public Transform squashTarget = null;

		// Andere Komponenten, die wir uns dafür merken müssen
		private Bodenkontakt bodenkontakt;
		private Animator     animator;
		private Rigidbody2D  körper;

		// Variablen für den 'Squash & Stretch'-Effekt
		private Coroutine squashAndStretchCoroutine;
		private bool      stehtAufBoden;

		private bool moveParticlesWerdenAbgespielt = false;


		private void Start()
		{
			bodenkontakt = GetComponent<Bodenkontakt>();
			animator     = GetComponentInChildren<Animator>();
			körper       = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			SpriteDrehen();

			// Hier passen wir die Animations-Geschwindigkeit daran an, wie schnell der Charakter gerade tatsächlich rennt
			animator.SetFloat("geschwindigkeit", Mathf.Clamp(körper.velocity.magnitude / 4f, 0f, 10f));

			if(stehtAufBoden && !bodenkontakt.StehtAufDemBoden()) {
				stehtAufBoden = false;
			} else if(!stehtAufBoden && bodenkontakt.StehtAufDemBoden()) {
				stehtAufBoden = true;
				LandenEffekte();
			}

			if(moveParticles) {
				if(bodenkontakt.StehtAufDemBoden() && Mathf.Abs(körper.velocity.x) > 1f) {
					if(!moveParticlesWerdenAbgespielt) {
						moveParticles.Play();
						moveParticlesWerdenAbgespielt = true;
					}
				} else {
					if(moveParticlesWerdenAbgespielt) {
						moveParticles.Stop();
						moveParticlesWerdenAbgespielt = false;
					}
				}
			}
		}

		private void SpriteDrehen()
		{
			// Die Richtung in die wir drehen/kippen hängt von der Laufrichtung ab
			var tiltRichtung = körper.velocity.x == 0 ? 0f : -Mathf.Sign(körper.velocity.x);
			var t            = squashTarget ? squashTarget : animator.transform;
			t.rotation = Quaternion.RotateTowards(t.rotation, Quaternion.Euler(0, 0, leanAngle * tiltRichtung),
			                                      leanSpeed * Time.deltaTime);
		}

		private void LandenEffekte()
		{
			// Die passende Animation abspielen
			animator.SetTrigger("gelandet");

			// ggf. den festgelegten Soundeffekt abspielen
			if(landSoundEffect && !landSoundEffect.isPlaying && landSoundEffect.enabled) landSoundEffect.Play();

			// Partikel für Staubwolke erzeugen
			if(landParticles && !bodenkontakt.StehtAnEinerKante()) landParticles.Play();

			if (squashAndStretch) {
				// Squash & Stretch Effekt starten
				SquashAndStretchStarten(landSquashSettings, landDrop, landSqueezeMultiplier);
			}
		}

		public void SchadensEffekteStarten() { animator.SetBool("schaden", true); }
		public void SchadensEffekteBeenden() { animator.SetBool("schaden", false); }

		public void SprungEffekte()
		{
			// Die passende Animation abspielen
			animator.ResetTrigger("gelandet");
			animator.SetTrigger("springen");

			// ggf. den festgelegten Soundeffekt abspielen
			if(jumpSoundEffect && jumpSoundEffect.enabled) jumpSoundEffect.Play();

			// Partikel für Staubwolke erzeugen
			if(jumpParticles) jumpParticles.Play();

			if (squashAndStretch) {
				// Squash & Stretch Effekt starten
				SquashAndStretchStarten(jumpSquashSettings, 0, jumpSqueezeMultiplier);
			}
		}

		private void SquashAndStretchStarten(SquashSettings einstellungen, float drop, float faktor)
		{
			if(faktor < 1f) return;

			// Der eigentliche Effekt passiert über die Coroutine unten über mehrere Frames
			if(squashAndStretchCoroutine != null) StopCoroutine(squashAndStretchCoroutine);
			squashAndStretchCoroutine = StartCoroutine(SquashAndStretch(einstellungen.breiteFaktor,  einstellungen.höheFaktor,
			                                                            einstellungen.dauerSekunden, drop, faktor - 1));
		}

		private IEnumerator SquashAndStretch(float xFaktor,
		                                     float yFaktor,
		                                     float dauerSekunden,
		                                     float drop,
		                                     float stärke)
		{
			var visualTransform = squashTarget ? squashTarget : animator.transform;
		
			var verzerrteGröße    = Vector3.LerpUnclamped(Vector3.one, new Vector3(xFaktor, yFaktor, 1f), stärke);
			var verzerrtePosition = new Vector3(0, -drop * stärke, 0);

			// Zuerst verzerren wir das Sprite schnell (in 0.1 Sekunden) auf die gewünschten Maße
			for(var t = 0f; t <= 1f; t += Time.deltaTime / 0.1f) {
				visualTransform.localScale    = Vector3.Lerp(Vector3.one,  verzerrteGröße,    t);
				visualTransform.localPosition = Vector3.Lerp(Vector3.zero, verzerrtePosition, t);
				yield return null; // (nach jeder Änderung der Maße warten wir bis zum nächsten Frame)
			}

			visualTransform.localScale    = verzerrteGröße;
			visualTransform.localPosition = verzerrtePosition;

			// ... und danach strecken wir es langsam, über die festgelegte Dauer, wieder auf seine ursprünglichen Maße
			for(var t = 0f; t <= 1f; t += Time.deltaTime / dauerSekunden) {
				visualTransform.localScale    = Vector3.Lerp(verzerrteGröße,    Vector3.one,  t);
				visualTransform.localPosition = Vector3.Lerp(verzerrtePosition, Vector3.zero, t);
				yield return null; // (nach jeder Änderung der Maße warten wir bis zum nächsten Frame)
			}

			visualTransform.localScale    = Vector3.one;
			visualTransform.localPosition = Vector3.zero;

			squashAndStretchCoroutine = null;
		}
	}
}
