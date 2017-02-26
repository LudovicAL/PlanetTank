using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public GameObject spawnPrefab;
	private GameObject[] planetList;
	private int nextPlayerId;
	[SyncVar]
	private int currentPlanet;

	void Awake() {
		nextPlayerId = 0;
		planetList = GameObject.FindGameObjectsWithTag ("Planet");
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void RandomizePlanet () {
		if (isServer) {
			currentPlanet = Random.Range (0, planetList.Length - 1);
			GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Spawn");
			MoveSpawn (spawns [0], new Vector3 (10.0f, 0.0f, 0.0f));
			MoveSpawn (spawns [1], new Vector3 (-10.0f, 0.0f, 0.0f));
			MoveSpawn (spawns [2], new Vector3 (0.0f, 10.0f, 0.0f));
			MoveSpawn (spawns [3], new Vector3 (0.0f, -10.0f, 0.0f));
			MoveSpawn (spawns [4], new Vector3 (0.0f, 0.0f, 10.0f));
			MoveSpawn (spawns [5], new Vector3 (0.0f, 0.0f, -10.0f));
			MoveSpawn (spawns [6], new Vector3 (10.0f, 10.0f, 0.0f));
			MoveSpawn (spawns [7], new Vector3 (-10.0f, -10.0f, 0.0f));
		}
	}

	private void MoveSpawn(GameObject spawn, Vector3 modifier) {
		spawn.transform.position = GetPlanet ().transform.position + modifier;
		spawn.transform.rotation = Quaternion.LookRotation (spawn.transform.position - GetPlanet ().transform.position + new Vector3(0.0f, -90.0f, 0.0f));
	}

	public GameObject GetPlanet() {
		return planetList [currentPlanet];
	}

	public int GeneratePlayerId() {
		nextPlayerId++;
		return nextPlayerId;
	}
}
