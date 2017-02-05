using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HeadController : NetworkBehaviour {

	public float rotationSpeed;
	public float maxAngle;
	public float canonCoolDownDuration;
	public float canonBallForce;
	public float recoilForce;
	public GameObject ShootingFXPrefab;
	public GameObject canonBallPrefab;
	private GameObject head;
	private GameObject canonBase;
	private GameObject canonTip;
	private GameObject canon;
	private Rigidbody selfRigidbody;
	private AudioSource audioSource;
	private SmoothFollow cameraSmoothFollow;
	private float remainingCoolDown = 0.0f;


	// Use this for initialization
	void Start () {
		head = transform.FindChild("Meshes").FindChild("Head").gameObject;
		canonBase = head.transform.FindChild("CanonBase").gameObject;
		canon = canonBase.transform.FindChild("Canon").gameObject;
		canonTip = canon.transform.FindChild("CanonTip").gameObject;
		selfRigidbody = this.GetComponent<Rigidbody>();
		cameraSmoothFollow = Camera.main.GetComponent<SmoothFollow> ();
		audioSource = canon.GetComponent<AudioSource>();
		if (isLocalPlayer) {
			cameraSmoothFollow.target = head.transform;
			cameraSmoothFollow.UpdatePlanet ();
			UpdateCanonColor();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				RotateLeftRight (hit);
				RotateUpDown (hit);
			}
			if (Input.GetButtonDown("Fire1")) {
				FireCanon();
			}
			UpdateCanonCoolDown();
		}
	}

	public void LateUpdate() {
		if (isLocalPlayer) {
			cameraSmoothFollow.UpdateCameraPosition();
		}
	}

	public void UpdateCanonCoolDown() {
		if (remainingCoolDown > 0.0f) {
			remainingCoolDown -= Time.fixedDeltaTime;
			remainingCoolDown = Mathf.Max(remainingCoolDown, 0.0f);
			UpdateCanonColor();
		}
	}

	public void UpdateCanonColor() {
		MeshRenderer meshRenderer = canon.GetComponent<MeshRenderer>();
		if (meshRenderer != null) {
			Color cannonColor = remainingCoolDown > 0.0f ? Color.red : Color.black;
			meshRenderer.material.color = cannonColor;
		}
	}

	public void FireCanon() {
		if (remainingCoolDown <= 0.0f) {
			//Recoil
			selfRigidbody.AddForce(canon.transform.forward * -recoilForce);
			//Canonball
			GameObject newCannonball = GameObject.Instantiate(canonBallPrefab, canonTip.transform.position, Quaternion.identity, null);
			newCannonball.GetComponent<Rigidbody>().AddForce(canon.transform.forward * canonBallForce, ForceMode.Impulse);
			//Sound
			audioSource.Play();
			//Particle Effect
			GameObject spawnedFX = GameObject.Instantiate(ShootingFXPrefab, canonTip.transform.position, canon.transform.rotation, canonTip.transform);
			spawnedFX.transform.localPosition = Vector3.zero;
			//Cooldown
			remainingCoolDown = canonCoolDownDuration;
		}
	}

	public void RotateLeftRight(RaycastHit hit) {
		Vector3 direction = (head.transform.InverseTransformPoint (hit.point) - head.transform.localPosition);
		direction.y = 0.0f;
		direction.z = 0.0f;
		if (direction.magnitude > 0.1f) {
			Quaternion lookRotation = Quaternion.LookRotation (direction);
			head.transform.localRotation = Quaternion.RotateTowards (head.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
		}
	}

	public void RotateUpDown(RaycastHit hit) {
		Vector3 direction = (canonBase.transform.InverseTransformPoint (hit.point) - canonBase.transform.localPosition);
		if (direction.y > 0.1f) {	//Raise the canon
			if (canonBase.transform.localEulerAngles.x < 180.0f || Vector3.Angle(head.transform.forward, canonBase.transform.forward) < maxAngle) {
				direction.x = 0.0f;
				direction.z = 0.0f;
				Quaternion lookRotation = Quaternion.LookRotation (direction);
				canonBase.transform.localRotation = Quaternion.RotateTowards (canonBase.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
			}
		} else if (direction.y < -0.1f) {	//Lower the canon
			if (canonBase.transform.localEulerAngles.x > 180.0f || Vector3.Angle(head.transform.forward, canonBase.transform.forward) < maxAngle) {
				direction.x = 0.0f;
				direction.z = 0.0f;
				Quaternion lookRotation = Quaternion.LookRotation (direction);
				canonBase.transform.localRotation = Quaternion.RotateTowards (canonBase.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
			}
		}

	}
}