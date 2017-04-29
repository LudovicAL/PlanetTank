using UnityEngine;

public class FaceCamera : MonoBehaviour {
	//Makes an object face the main camera at all time
	void Update () {
		this.transform.LookAt (Camera.main.transform.position);
		this.transform.Rotate (new Vector3 (0.0f, 180.0f, 0.0f));
	}
}
