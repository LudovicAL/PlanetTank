using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	[Tooltip("The spawn prefab instantiated all around a planet to mark the different player starting positions.")] public GameObject spawnPrefab;
	[HideInInspector]
	public List<GameObject> tankList;
	private GameObject[] planetList;
	private GameObject[] spawnList;
	private List<Vector3> spawnOffsets;
	private int nextAvailableSpawn;
	[SyncVar]
	private int currentPlanet = -1;

	void Awake() {
		planetList = GameObject.FindGameObjectsWithTag ("Planet");
		if (!isClient) {
			RandomizePlanet ();
		}
	}

	void Start () {
		GameObject.Find ("GameStatusPanel").GetComponent<GameStatusManager> ().enabled = true;
		nextAvailableSpawn = 0;
		tankList = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Player"));

		InitializeSpawns ();
		foreach (GameObject go in tankList) {
			go.GetComponent<Activator> ().Activate ();
		}
		Camera.main.GetComponent<Activator> ().Activate ();
	}

	/// <summary>
	/// Generates a random number used to identify the planet that will be used for the current game
	/// </summary>
	private void RandomizePlanet() {
		if (currentPlanet == -1) {
			currentPlanet = Random.Range (0, planetList.Length);
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
	public void MovePlayerToHisSpawn(GameObject player) {
		GameObject spawn = spawnList[nextAvailableSpawn];
		nextAvailableSpawn++;
		if (nextAvailableSpawn >= spawnList.Length) {
			nextAvailableSpawn = 0;
		}
		player.GetComponent<HeadController>().RpcSetSpawn (spawn.transform.position);
	}
}
