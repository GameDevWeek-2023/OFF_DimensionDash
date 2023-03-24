using Movement;
using Player;
using UnityEngine;

namespace Skripte.Bewegung
{
    public class TeleportierenBewegung : BewegenÜberschreiben
    {
	    [SerializeField] private GameObject   crosshair;
	    [SerializeField] private Vector2      zielpunkt;
        public                   bool         teleportieren   = false;
        private                  Vector2      richtung        = Vector2.zero;
        private                  RaycastHit2D hit;
        private                  bool         läuft   = false;
        [SerializeField] private                  float        distanz = 8;
        
        [SerializeField] private PlayerColor _playerColor;
        
        private                  GameObject  crosshairInstance;
        
        public override bool WennLaufen(Vector2 richtung)
        {
	        this.richtung = richtung;
	        if (teleportieren)
	        {
		        int layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther");
		        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), richtung, distanz, layer_mask);
		        if (!crosshairInstance)
		        {
			        crosshairInstance = Instantiate(crosshair, zielpunkt, Quaternion.identity);
			        if(_playerColor && _playerColor.GetColor() && crosshairInstance.TryGetComponent(out SpriteRenderer sprite)) {
				        sprite.color = _playerColor.GetColor().Color;
			        }
		        }
		        
		        if (!hit.collider)
		        {
			        zielpunkt = new Vector2(transform.position.x, transform.position.y) + richtung * distanz;
			        
		        } else
		        {
			        zielpunkt = hit.point;
		        }
		        crosshairInstance.transform.position = zielpunkt;
		        return false;
	        }

	        return true;
        }

        public override bool WennAktuallisieren()
        {
	        if (crosshairInstance)
	        {
		        if (richtung.sqrMagnitude > 0.001f && teleportieren)
		        {
			        crosshairInstance.SetActive(true);
		        } else
		        {
			        crosshairInstance.SetActive(false);
		        }
	        }
	        return true;
        }
        
        public override bool WennSpringen()
        {
	        if (this.enabled && teleportieren)
	        {
		        transform.position = zielpunkt;
		        return false;
	        }
	        return true;
        }
        
        private void Update() {
	        // required to have enable
        }
    }
}
