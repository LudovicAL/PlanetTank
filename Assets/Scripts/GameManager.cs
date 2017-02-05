using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject currentPlanet { get; private set; }
	private GameObject[] planetList;

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
