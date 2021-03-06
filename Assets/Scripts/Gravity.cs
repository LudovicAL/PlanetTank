﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

	private float gravity = -100000.0f;
	private Rigidbody rb;
	private GameObject planet;

	void Start () {
		rb = this.GetComponent<Rigidbody> ();
		planet = GameObject.Find ("ScriptsBucket").GetComponent<GameManager> ().GetPlanet();
	}

	void FixedUpdate () {
		rb.AddForce((this.transform.position - planet.transform.position).normalized * gravity * Time.fixedDeltaTime);
	}
}
