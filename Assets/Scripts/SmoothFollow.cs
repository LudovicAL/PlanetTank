using UnityEngine;

public class SmoothFollow : MonoBehaviour {


	[HideInInspector] public Transform target;
	[Tooltip("Distance to the target.")] public float distance;
	[Tooltip("Height relative to the target.")] public float height;
	[Tooltip("Angle of the camera.")] public float angle;
	private GameObject planet;
	[Tooltip("Damping applied on the camera rotation speed.")] public float rotationDamping;
	[Tooltip("Damping applied on the camera movement speed.")] public float positionDamping;

	void Start() {
		
	}

	/// <summary>
	/// Updates the camera's reference to the active planet GameObject.
	/// </summary>
	public void UpdatePlanet() {
		planet = GameObject.Find ("ScriptsBucket").GetComponent<GameManager> ().GetPlanet();
	}

	/// <summary>
	/// Updates the camera position relative to the player and the active planet. This function should be called during late update.
	/// </summary>
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

