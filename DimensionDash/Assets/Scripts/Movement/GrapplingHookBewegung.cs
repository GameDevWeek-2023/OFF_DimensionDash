using System;
using Movement;
using UnityEngine;

namespace Skripte.Bewegung {
	public class GrapplingHookBewegung : BewegenÜberschreiben {
		public  bool    grapplinghook = false;
		private bool    zieht         = false;
		private Vector2 richtung;

		[SerializeField] private float   speed = 30f;
		[SerializeField] private Vector2 zielpunkt;

		public override bool WennLaufen(Vector2 richtung) {
			this.richtung = richtung;
			return true;
		}

		public override bool WennSpringen() {
			if (this.enabled && grapplinghook) {
				Ziehen(richtung);
				return false;
			}

			return true;
		}

		public override bool WennAktuallisieren() {
			if (!grapplinghook) {
				zieht     = false;
				zielpunkt = Vector2.zero;
			}

			if (this.enabled && grapplinghook && zieht) {
				Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
				if ((playerPosition - zielpunkt).sqrMagnitude > 1) {
					Vector2 pos = Vector2.MoveTowards(transform.position, zielpunkt, speed * Time.deltaTime);
					this.GetComponent<Rigidbody2D>().MovePosition(pos);
				} else {
					zieht     = false;
					zielpunkt = Vector2.zero;
				}
			}

			return true;
		}

		private void Ziehen(Vector2 richtung) {
			if (this.enabled && grapplinghook && richtung.sqrMagnitude > 0.01f) {
				if (!grapplinghook || !zieht) {
					Vector2      playerPosition = new Vector2(transform.position.x, transform.position.y);
					int          layer_mask     = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
					RaycastHit2D hit            = Physics2D.Raycast(playerPosition, richtung, 40, layer_mask);
					if (hit && hit.collider != null) {
						zieht     = true;
						zielpunkt = hit.point;
					}
				}
			}
		}
	}
}
