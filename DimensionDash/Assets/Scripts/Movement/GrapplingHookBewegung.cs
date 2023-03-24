using System;
using Movement;
using Player;
using UnityEngine;

namespace Skripte.Bewegung
{
	public class GrapplingHookBewegung : BewegenÜberschreiben
	{
		[SerializeField] private GameObject   crosshair;
		public                   bool         grapplinghook   = false;
		private                  bool         zieht           = false;
		private                  Vector2      richtung;
		private                  RaycastHit2D hit;
		[SerializeField] private                  float        distance  = 25;
		private                  bool         plattform = false;
		[SerializeField] private float        speed     = 30f;
		[SerializeField] private Vector2      zielpunkt;
		[SerializeField] private GameObject   cordGrabblingHook;

		[SerializeField] private PlayerColor _playerColor;
		
		private                  GameObject cordGrabblingHookInstance;
		private GameObject crosshairInstance;

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
				if (!crosshairInstance && hit.collider)
				{
					crosshairInstance = Instantiate(crosshair, zielpunkt, Quaternion.identity);
					if(_playerColor && _playerColor.GetColor() && crosshairInstance.TryGetComponent(out SpriteRenderer sprite)) {
						sprite.color = _playerColor.GetColor().Color;
					}
				}

				if (crosshairInstance) {
					if (!crosshairInstance.activeSelf && hit.collider) {
						crosshairInstance.SetActive(true);
					}

					crosshairInstance.transform.position = zielpunkt;
				}
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

		private void OnEnable()
		{
			if (cordGrabblingHookInstance)
			{
				cordGrabblingHookInstance.SetActive(true);
			}
		}

		public override bool WennAktuallisieren()
		{
			if (crosshairInstance)
			{
				if (richtung.sqrMagnitude > 0.001f && grapplinghook)
				{
					crosshairInstance.SetActive(true);	
				} else
				{
					crosshairInstance.SetActive(false);
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
			if (!cordGrabblingHookInstance)
			{
				cordGrabblingHookInstance       = Instantiate(cordGrabblingHook, center, Quaternion.identity);
			}
			cordGrabblingHookInstance.transform.position =  center;
			center.x                                     -= 0.5f;
			center.y                                     += 0.5f;
			cordGrabblingHookInstance.transform.position =  center;
			Vector2 direction = zielpunkt - playerPosition;
			cordGrabblingHookInstance.transform.localScale = new Vector3(direction.magnitude * 0.1f, 1, 0);
			Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
			cordGrabblingHookInstance.transform.rotation = rotation;
		}
		
		private void grapplingHookMechanic(Vector2 playerPosition) {
			if (!cordGrabblingHookInstance)
				return;
			
			float radius = 1f;
			if (plattform)
			{
				radius = 0.1f;
			}
			if ((playerPosition - zielpunkt).sqrMagnitude > radius)
			{
				Vector2 pos = Vector2.MoveTowards(transform.position, zielpunkt, speed * Time.deltaTime);
				this.GetComponent<Rigidbody2D>().MovePosition(pos);
				cordGrabblingHookInstance.gameObject.SetActive(true);
			} else
			{
				cordGrabblingHookInstance.gameObject.SetActive(false);
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
			if(cordGrabblingHookInstance)
				cordGrabblingHookInstance.SetActive(false);
			
		}
	}
}
