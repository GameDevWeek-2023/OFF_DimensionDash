using System;
using UnityEngine;

public class FollowMainCamera : MonoBehaviour {
	private void LateUpdate() {
		var cam = Camera.main;
		if (cam)
			transform.position = cam.transform.position;
		else
			transform.position = Vector3.zero;
	}
}
