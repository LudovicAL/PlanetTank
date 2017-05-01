using UnityEngine;
//This scripts makes an object face the main camera at all time
public class FaceCamera : MonoBehaviour {
	void Update () {
		this.transform.LookAt (Camera.main.transform.position);
		this.transform.Rotate (new Vector3 (0.0f, 180.0f, 0.0f));
	}
}