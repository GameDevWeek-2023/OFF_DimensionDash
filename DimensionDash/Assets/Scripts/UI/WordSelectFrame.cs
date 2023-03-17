using System.Collections;
using System.Collections.Generic;
using Dimensions;
using UnityEngine;
using UnityEngine.UI;

public class WordSelectFrame : MonoBehaviour
{

	[SerializeField]
	private Image myIcon;
	[SerializeField]
	private Image worldSelectionFrameX;
	[SerializeField]
	private DimensionSelection dimensionSelection;
	[SerializeField]
	private int myDimension;

	private Sprite mySpriteGrey;
	private Sprite mySprite;

	public void ClickMe() 
	{
		if (dimensionSelection.EnabledDimensions[myDimension]) {
			dimensionSelection.EnabledDimensions[myDimension] = false;
			myIcon.sprite = mySpriteGrey;
			worldSelectionFrameX.gameObject.SetActive(true);
		} else {
			dimensionSelection.EnabledDimensions[myDimension] = true;
			myIcon.sprite = mySprite;
			worldSelectionFrameX.gameObject.SetActive(false);
		}
	}


    void Awake()
    {
		mySprite = dimensionSelection.Dimensions[myDimension].Icon;
		mySpriteGrey = dimensionSelection.Dimensions[myDimension].GreyIcon;
		worldSelectionFrameX.gameObject.SetActive(false);
		myIcon.sprite = mySprite;
    }



    // Update is called once per frame
    void Update()
    {
        
    }


}
