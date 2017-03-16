using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gravity : NetworkBehaviour {

	private float gravity = -100000.0f;
	private Rigidbody rb;
	private GameObject planet;

	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		planet = GameObject.Find ("ScriptsBucket").GetComponent<GameManager> ().GetPlanet();
	}

	void FixedUpdate () {
		if (isLocalPlayer) {
			rb.AddForce((this.transform.position - planet.transform.position).normalized * gravity * Time.fixedDeltaTime);
		}
	}
}
