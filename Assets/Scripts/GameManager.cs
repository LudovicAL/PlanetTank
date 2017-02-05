using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	private GameObject[] planetList;
	public GameObject currentPlanet { get; private set; }

	void Awake() {
			planetList = GameObject.FindGameObjectsWithTag ("Planet");
			currentPlanet = GetRandomPlanet ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private GameObject GetRandomPlanet () {
		int i = Random.Range (0, planetList.Length - 1); 
		return planetList [i];
	}
}
