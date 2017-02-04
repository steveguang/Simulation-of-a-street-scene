using UnityEngine;
using System.Collections;

public class NavMeshObstacleScript : MonoBehaviour {
	public Plane groundPlane;

	void Start() {
		groundPlane = new Plane(Vector3.up, new Vector3(0,1,0));
	}

	void OnMouseDrag() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float rayDistance;
		if (groundPlane.Raycast(ray, out rayDistance)) {
			this.transform.position = ray.GetPoint(rayDistance);
		}
	}
}
