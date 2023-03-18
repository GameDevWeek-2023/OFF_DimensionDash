using System;
using Movement;
using UnityEngine;

namespace Skripte.Bewegung
{
	public class GrapplingHookBewegung : BewegenÜberschreiben
	{
		[SerializeField] private GameObject   crosshair;
		private                  bool         crosshairExists = false;
		public                   bool         grapplinghook   = false;
		private                  bool         zieht           = false;
		private                  Vector2      richtung;
		private                  RaycastHit2D hit;

		[SerializeField] private float   speed = 30f;
		[SerializeField] private Vector2 zielpunkt;

		public override bool WennLaufen(Vector2 richtung)
		{
			if (grapplinghook)
			{
				int          layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
				hit       = Physics2D.Raycast(transform.position, richtung, 10, layer_mask);
				if (hit.point != Vector2.zero)
				{
					zielpunkt = hit.point;
				}
				if (!crosshairExists && hit.collider)
				{
					crosshair       = Instantiate(crosshair, zielpunkt, Quaternion.identity);
					crosshairExists = true;
				}

				if (!crosshair.activeSelf && hit.collider)
				{
					crosshair.SetActive(true);
				}
				crosshair.transform.position = hit.point;
			}
			return true;
		}

		public override bool WennSpringen()
		{
			if (grapplinghook && hit.collider && this.enabled && !zieht)
			{
				zieht = true;
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
				}
			}

			if (richtung.sqrMagnitude > 0.001f)
			{
				if (crosshairExists)
				{
					crosshair.SetActive(true);
				} 
			} else
			{
				if (crosshairExists)
				{
					crosshair.SetActive(false);
				}
			}
			return true;
		}
	}
}
