using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class thomas : MonoBehaviour
{
	[SerializeField]
	private List<Sprite> thomaseseses;
	[SerializeField]
	private float thomasTime;

	private int myThomas;
	private float thomasTimer;


	private void Awake() {
		thomasTimer = thomasTime;
		thomasTime = (60f / 90 / 8);
	}

    // Update is called once per frame
    void Update()
    {
        if(thomasTimer > 0) {
			thomasTimer -= Time.deltaTime;

			if(thomasTimer <= 0) {
				thomasTimer = thomasTime;
				if (thomaseseses[myThomas + 1] != null)
					myThomas += 1;
				else
					myThomas = 0;
					
				GetComponent<Image>().sprite = thomaseseses[myThomas];
			}
		}
    }
}
