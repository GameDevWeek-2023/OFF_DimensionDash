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
		private                  float        distance = 10;
		private                  float        oldtime;

		[SerializeField] private float   speed = 30f;
		[SerializeField] private Vector2 zielpunkt;

		public override bool WennLaufen(Vector2 richtung)
		{
			if (grapplinghook)
			{
				int          layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
				hit       = Physics2D.Raycast(transform.position, richtung, distance, layer_mask);
				if (hit.collider)
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
				crosshair.transform.position = zielpunkt;
			}
			return true;
		}

		public override bool WennSpringen()
		{
			if (grapplinghook && hit.collider && this.enabled && !zieht)
			{
				zieht = true;
				return false;
			} else if (grapplinghook && this.enabled && zieht)
			{
				//im ziehen springen, ziehen abbrechen
				zieht = false;
			}
			return true;
		}

		public override bool WennAktuallisieren()
		{
			//check if respawned, then break
			Debug.Log(Time.time - oldtime);
			if (Time.time - oldtime > 1.9f)
			{
				zieht = false;
			}

			oldtime = Time.time;
			
			//grappling hook
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

			if (richtung.sqrMagnitude > 0.001f && grapplinghook)
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
