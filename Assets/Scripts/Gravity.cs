using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

	private float gravity = -100009.81f;
	private Rigidbody rb;
	private GameObject planet;

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		planet = GameObject.Find ("ScriptsBucket").GetComponent<GameManager> ().currentPlanet;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rb.AddForce((this.transform.position - planet.transform.position).normalized * gravity * Time.fixedDeltaTime);
	}
}
