using UnityEngine;

public class SmoothFollow : MonoBehaviour {

	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance;
	public float height;
	public float angle;
	public GameObject planet;
	public float rotationDamping;
	public float positionDamping;

	// Use this for initialization
	void Start() { }

	// Update is called once per frame
	void LateUpdate() {
		if (target) {
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