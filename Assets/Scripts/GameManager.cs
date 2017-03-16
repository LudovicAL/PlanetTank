using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	[Tooltip("The spawn prefab instantiated all around a planet to mark the different player starting positions.")] public GameObject spawnPrefab;
	private GameObject[] planetList;
	[SyncVar]
	private int currentPlanet;

	void Awake() {
		planetList = GameObject.FindGameObjectsWithTag ("Planet");
	}

	/// <summary>
	/// Selects a random planet for the next match and move the spawn position all around it.
	/// </summary>
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

	/// <summary>
	/// Moves a spawn to a specified location.
	/// </summary>
	private void MoveSpawn(GameObject spawn, Vector3 modifier) {
		spawn.transform.position = GetPlanet ().transform.position + modifier;
		spawn.transform.rotation = Quaternion.LookRotation (spawn.transform.position - GetPlanet ().transform.position + new Vector3(0.0f, -90.0f, 0.0f));
	}

	/// <summary>
	/// Return the planet selected for the current match.
	/// </summary>
	public GameObject GetPlanet() {
		return planetList [currentPlanet];
	}
}
