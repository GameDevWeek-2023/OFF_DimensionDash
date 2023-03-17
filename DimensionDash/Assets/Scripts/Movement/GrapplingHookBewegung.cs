using System;
using Movement;
using UnityEngine;

namespace Skripte.Bewegung
{
	public class GrapplingHookBewegung : BewegenÜberschreiben
	{
		public                   bool    grapplinghook = false;
		private                  bool    zieht         = false;
		private                  Vector2 richtung;
		
		[SerializeField] private float   speed = 30f;
		[SerializeField] private Vector2 zielpunkt;

		public override bool WennLaufen(Vector2 richtung)
		{
			this.richtung = richtung;
			return true;
		}

		public override bool WennSpringen()
		{
			if (grapplinghook)
			{
				Ziehen(richtung);
				return false;
			}
			return true;
		}

		public override bool WennAktuallisieren()
		{
			Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
			if (this.enabled && grapplinghook && zieht)
			{
				if ((playerPosition - zielpunkt).sqrMagnitude>1)
				{
					Vector2 pos = Vector2.MoveTowards(transform.position, zielpunkt, speed * Time.deltaTime);
					this.GetComponent<Rigidbody2D>().MovePosition(pos);
				} else
				{
					zieht = false;
					zielpunkt = Vector2.zero;
				}
			}
			return true;
		}

		private void Ziehen(Vector2 richtung)
		{
			Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
			if (this.enabled && grapplinghook)
			{
				if (!grapplinghook || !zieht)
				{
					int          layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
					RaycastHit2D hit        = Physics2D.Raycast(playerPosition, richtung, 1000, layer_mask);
					if (hit.collider != null)
					{
						zieht     = true;
						zielpunkt = hit.point;
					}
				}
			}
		}
	}
}
