using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	[Tooltip("The spawn prefab instantiated all around a planet to mark the different player starting positions.")] public GameObject spawnPrefab;
	private GameObject[] planetList;
	private GameObject[] spawnList;
	private GameObject[] tankList;
	private List<Vector3> spawnOffsets;
	private int nextAvailableSpawn;
	[SyncVar]
	private int currentPlanet;

	void Start () {
		nextAvailableSpawn = 0;
		tankList = GameObject.FindGameObjectsWithTag ("Player");
		RandomizePlanet ();
		InitializeSpawns ();
		foreach (GameObject go in tankList) {
			go.GetComponent<HeadController> ().InitializeGameManager ();
		}
		Camera.main.GetComponent<SmoothFollow> ().InitializeGameManager ();
	}


	/// <summary>
	/// Selects a random planet for the next match and move the spawn position all around it.
	/// </summary>
	public void RandomizePlanet () {
		planetList = GameObject.FindGameObjectsWithTag ("Planet");
		if (isServer) {
			currentPlanet = Random.Range (0, planetList.Length - 1);
		}
	}

	/// <summary>
	/// Initializes the spawns
	/// </summary>
	private void InitializeSpawns() {
		if (isServer) {
			spawnList = GameObject.FindGameObjectsWithTag ("Spawn");
			spawnOffsets = new List<Vector3> ();
			spawnOffsets.Add (new Vector3 (10.0f, 0.0f, 0.0f));
			spawnOffsets.Add (new Vector3 (-10.0f, 0.0f, 0.0f));
			spawnOffsets.Add (new Vector3 (0.0f, 10.0f, 0.0f));
			spawnOffsets.Add (new Vector3 (0.0f, -10.0f, 0.0f));
			spawnOffsets.Add (new Vector3 (0.0f, 0.0f, 10.0f));
			spawnOffsets.Add (new Vector3 (0.0f, 0.0f, -10.0f));
			spawnOffsets.Add (new Vector3 (10.0f, 10.0f, 0.0f));
			spawnOffsets.Add (new Vector3 (-10.0f, -10.0f, 0.0f));
			spawnOffsets.Shuffle ();
			for (int i = 0, maxA = spawnList.Length, maxB = spawnOffsets.Count; i < maxA && i < maxB; i++) {
				MoveSpawn (i);
			}
		}
	}

	/// <summary>
	/// Moves a spawn to a specified location.
	/// </summary>
	private void MoveSpawn(int spawnNumber) {
		spawnList[spawnNumber].transform.position = GetPlanet ().transform.position + spawnOffsets[spawnNumber];
		spawnList[spawnNumber].transform.rotation = Quaternion.LookRotation (spawnList[spawnNumber].transform.position - GetPlanet ().transform.position + new Vector3(0.0f, -90.0f, 0.0f), spawnList[spawnNumber].transform.position - GetPlanet ().transform.position);
	}

	/// <summary>
	/// Return the planet selected for the current match.
	/// </summary>
	public GameObject GetPlanet() {
		return planetList [currentPlanet];
	}

	/// <summary>
	/// Returns the next available spawn position.
	/// </summary>
	[Command]
	public void CmdMovePlayerToHisSpawn(GameObject player) {
		GameObject spawn = spawnList[nextAvailableSpawn];
		nextAvailableSpawn++;
		if (nextAvailableSpawn >= spawnList.Length) {
			nextAvailableSpawn = 0;
		}
		player.GetComponent<HeadController>().RpcMoveToSpawn (spawn.transform.position, spawn.transform.rotation);
	}
}
