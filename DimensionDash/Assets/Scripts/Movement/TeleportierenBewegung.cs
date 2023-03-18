using Movement;
using UnityEngine;

namespace Skripte.Bewegung
{
    public class TeleportierenBewegung : BewegenÜberschreiben
    {
	    [SerializeField] private GameObject   crosshair;
	    [SerializeField] private Vector2      zielpunkt;
	    private                  bool         crosshairExists = false;
        public                   bool         teleportieren   = false;
        private                  Vector2      richtung        = Vector2.zero;
        private                  RaycastHit2D hit;
        private                  bool         läuft   = false;
        [SerializeField] private                  float        distanz = 4;
        
        public override bool WennLaufen(Vector2 richtung)
        {
	        this.richtung = richtung;
	        if (teleportieren)
	        {
		        int layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
		        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), richtung, distanz, layer_mask);
		        if (!crosshairExists)
		        {
			        crosshairExists = true; 
			        crosshair = Instantiate(crosshair, zielpunkt, Quaternion.identity);
		        }
		        
		        if (!hit.collider)
		        {
			        zielpunkt = new Vector2(transform.position.x, transform.position.y) + richtung * distanz;
			        
		        } else
		        {
			        zielpunkt = hit.point;
		        }
		        crosshair.transform.position = zielpunkt;
		        return false;
	        }

	        return true;
        }

        public override bool WennAktuallisieren()
        {
	        if (crosshairExists)
	        {
		        if (richtung.sqrMagnitude > 0.001f)
		        {
			        crosshair.SetActive(true);
		        } else
		        {
			        crosshair.SetActive(false);
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
    }
}
