using Movement;
using UnityEngine;

namespace Skripte.Bewegung
{
    public class TeleportierenBewegung : BewegenÜberschreiben
    {
	    [SerializeField] private GameObject   crosshair;
	    private                  bool         crosshairExists = false;
        public                   bool         teleportieren   = false;
        private                  Vector2      richtung        = Vector2.zero;
        private                  RaycastHit2D hit;
        private                  bool         läuft = false;
        private                  Vector2      zielpunkt;
        
        public override bool WennLaufen(Vector2 richtung)
        {
	        this.richtung = richtung;
	        if (teleportieren)
	        {
		        int layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
		        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), richtung, 10, layer_mask);
		        if (!crosshairExists)
		        {
			        crosshairExists = true; 
			        crosshair = Instantiate(crosshair, zielpunkt, Quaternion.identity);
		        }

		        if (!hit.collider)
		        {
			        zielpunkt = hit.point;
		        }

		        crosshair.transform.position = hit.point;
	        }
	        return true;
        }

        public override bool WennAktuallisieren()
        {
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
        
        public override bool WennSpringen()
        {
	        if (teleportieren)
	        {
		        Teleportieren();
		        return false;
	        }
	        return true;
        }
        
        private void Teleportieren()
        {
            if (this.enabled && teleportieren && hit.collider != null)
            {
	            transform.position = new Vector3(zielpunkt.x, zielpunkt.y, 0);
            }
        }
    }
}
