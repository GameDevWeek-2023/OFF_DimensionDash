using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	private float            time         = 0;
	private List<GameObject> _parents     = new List<GameObject>();
	private float            secondsPerDimensnion = 2.5f;
	void Start()
	{
		time = Time.time;
		_parents = getChildParentsfromParent(this.gameObject);
		disableAllChildren(this.gameObject);
	}
	
    void Update()
    {
	    time += Time.deltaTime;
	    int dimension = (int) (time / secondsPerDimensnion);
	    if (dimension < _parents.Count)
	    {
		    disableAllChildren(this.gameObject);
		    _parents[dimension].SetActive(true);
		    if (dimension+1 < _parents.Count)
		    {
			    _parents[dimension+1].SetActive(true);
		    }
		    if (dimension-1 > 0)
		    {
			    _parents[dimension-1].SetActive(true);
		    }
	    }
    }

    private List<GameObject> getChildParentsfromParent(GameObject parent)
    {
	    List<GameObject> parents     = new List<GameObject>();
	    Transform[]      allChildren = parent.GetComponentsInChildren<Transform>();
	    foreach (Transform obj in allChildren)
	    {
		    if (obj.transform.parent == parent.transform)
		    {
			    parents.Add(obj.transform.gameObject);
		    }
	    }
	    return parents;
    }
    
    private void disableAllChildren(GameObject parent)
    {
	    Transform[] allChildren = parent.GetComponentsInChildren<Transform>();
	    for (int i = 0; i < allChildren.Length; i++)
	    {
		    Transform child = allChildren[i];
		    if (child.transform.parent == parent.transform)
		    {
			    child.transform.gameObject.SetActive(false);
		    }  
	    }
    }
}
