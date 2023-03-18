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
		private                  float        distance  = 10;
		private                  bool         plattform = false;
		[SerializeField] private float        speed     = 30f;
		[SerializeField] private Vector2      zielpunkt;
		[SerializeField] private GameObject   cordGrabblingHook;
		private                  bool         cordGrabblingHookExists = false;

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
						plattform = true;
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
			
			if (this.enabled && grapplinghook && zieht)
			{
				Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
				grapplingHookSprite(playerPosition);
				grapplingHookMechanic(playerPosition);
			} else
			{
				reset();
			}
			return true;
		}

		private void grapplingHookSprite(Vector2 playerPosition)
		{
			Vector2 center = (playerPosition + zielpunkt) / 2;
			if (!cordGrabblingHookExists)
			{
				cordGrabblingHook       = Instantiate(cordGrabblingHook, center, Quaternion.identity);
				cordGrabblingHookExists = true;
			}
			cordGrabblingHook.transform.position = center;
			
			float scaleX = Mathf.Abs(playerPosition.x - zielpunkt.x);
			float scaleY = Mathf.Abs(playerPosition.y - zielpunkt.y);
				
			center.x                                    -= 0.5f;
			center.y                                    += 0.5f;
			cordGrabblingHook.transform.position      =  center;
			Vector2 direction = zielpunkt - playerPosition;
			cordGrabblingHook.transform.localScale = new Vector3(direction.magnitude * 0.1f, 1, 0);
			Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
			cordGrabblingHook.transform.rotation = rotation;
		}
		
		private void grapplingHookMechanic(Vector2 playerPosition)
		{
			float radius = 1f;
			if (plattform)
			{
				radius = 0.1f;
			}
			if ((playerPosition - zielpunkt).sqrMagnitude > radius)
			{
				Vector2 pos = Vector2.MoveTowards(transform.position, zielpunkt, speed * Time.deltaTime);
				this.GetComponent<Rigidbody2D>().MovePosition(pos);
				cordGrabblingHook.gameObject.SetActive(true);
			} else
			{
				cordGrabblingHook.gameObject.SetActive(false);
				reset();
			}
		}
		
		private void Update() {
			// required to have enable
		}

		private void reset()
		{
			plattform           = false;
			zieht               = false;
		}
	}
}
