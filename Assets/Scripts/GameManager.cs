using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	private GameObject[] planetList;
	[SyncVar]
	private int currentPlanet;

	void Awake() {
		planetList = GameObject.FindGameObjectsWithTag ("Planet");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RandomizePlanet () {
		currentPlanet = Random.Range (0, planetList.Length - 1);
	}

	public GameObject GetPlanet() {
		return planetList [currentPlanet];
	}
}
