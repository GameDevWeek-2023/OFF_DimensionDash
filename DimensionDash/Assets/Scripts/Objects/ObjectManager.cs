using System.Collections.Generic;
using UnityEngine;

namespace Objects {
	public class ObjectManager : MonoBehaviour
	{
		private                float            _time               = 0;
		private                List<GameObject> _parents            = new List<GameObject>();
		public static readonly float            SecondsPerDimension = 2.5f; //TODO: Specify time of the dimensions
		void Start()
		{
			_time    = Time.time;
			_parents = getChildParentsfromParent(this.gameObject);
			DisableAllChildren(this.gameObject);
		}
	
		void Update()
		{
			_time += Time.deltaTime;
			int dimension = (int) (_time / SecondsPerDimension);
			if (dimension < _parents.Count)
			{
				DisableAllChildren(this.gameObject);
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
    
		private void DisableAllChildren(GameObject parent)
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
}
