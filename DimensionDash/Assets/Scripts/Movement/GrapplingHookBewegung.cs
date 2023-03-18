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
		private                  bool         Plattform = false;
		[SerializeField] private float        speed = 30f;
		[SerializeField] private Vector2      zielpunkt;

		public override bool WennLaufen(Vector2 richtung)
		{
			if (grapplinghook && !zieht)
			{
				int          layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
				hit       = Physics2D.Raycast(transform.position, richtung, distance, layer_mask);
				if (hit.collider)
				{
					int onlyplattforms = LayerMask.NameToLayer("DimensionPlattform");
					if (hit.transform.gameObject.layer == onlyplattforms)
					{
						Plattform = true;
						zielpunkt = new Vector2(hit.point.x, hit.collider.GetComponent<Renderer>().bounds.max.y+1f);
					} else
					{
						zielpunkt = hit.point;
					}
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
				reset();
				return false;
			}
			return true;
		}

		private void OnDisable()
		{
			reset();
		}

		public override bool WennAktuallisieren()
		{
			Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);

			if (this.enabled && grapplinghook && zieht)
			{
				float radius = 1f;
				if (Plattform)
				{
					radius = 0.1f;
				}
				if ((playerPosition - zielpunkt).sqrMagnitude>radius)
				{
					Vector2 pos = Vector2.MoveTowards(transform.position, zielpunkt, speed * Time.deltaTime);
					this.GetComponent<Rigidbody2D>().MovePosition(pos);
				} else
				{
					reset();
				}
			} else
			{
				reset();
			}

			if (crosshairExists)
			{
				if (richtung.sqrMagnitude > 0.001f && grapplinghook)
				{
					crosshair.SetActive(true);	
				} else
				{
					crosshair.SetActive(false);
				}
			}
			return true;
		}
		private void Update() {
			// required to have enable
		}

		private void reset()
		{
			Plattform = false;
			zieht     = false;
		}
	}
}
