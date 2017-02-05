using UnityEngine;
using UnityEngine.Networking;

public class SmoothFollow : NetworkBehaviour {

	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance;
	public float height;
	public float angle;
	private GameObject planet;
	public float rotationDamping;
	public float positionDamping;

	void Start() {
	}

	public void UpdatePlanet() {
		planet = GameObject.Find ("ScriptsBucket").GetComponent<GameManager> ().currentPlanet;
	}

	public void UpdateCameraPosition() { //Should be called during late update
		if (target != null) {
			//Damp the position
			Vector3 direction = target.transform.position - planet.transform.position;
			Vector3 desiredPosition = planet.transform.position + direction * height - target.transform.forward * distance;
			this.transform.position = Vector3.Lerp(this.transform.position, desiredPosition, positionDamping * Time.deltaTime);

			//Damp the rotation
			Vector3 desiredRotation = target.position - this.transform.position;
			Vector3 newDir = Vector3.RotateTowards(transform.forward, desiredRotation, rotationDamping * Time.deltaTime, 0.0f);
			this.transform.rotation = Quaternion.LookRotation(newDir);
		}
	}
}

