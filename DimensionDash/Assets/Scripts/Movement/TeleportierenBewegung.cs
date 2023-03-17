using Movement;
using UnityEngine;

namespace Skripte.Bewegung
{
    public class TeleportierenBewegung : BewegenÜberschreiben
    {
        public  bool    teleportieren = false;
        private Vector2 richtung;
        
        public override bool WennLaufen(Vector2 richtung)
        {
            this.richtung = richtung;
            return true;
        }

        public override bool WennSpringen()
        {
	        if (teleportieren)
	        {
		        Teleportieren(richtung);
		        return false;
	        }
	        return true;
        }
        
        private void Teleportieren(Vector2 richtung)
        {
            if (this.enabled && teleportieren)
            {
	            int          layer_mask = LayerMask.GetMask("BaseLevel", "DimensionOther", "DimensionPlattform");
	            RaycastHit2D hit        = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), richtung, 40, layer_mask);
	            if (hit && hit.collider != null)
	            {
		            transform.position = new Vector3(hit.point.x, hit.point.y, 0);
	            }
            }
        }
    }
}
