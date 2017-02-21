using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CannonBall : NetworkBehaviour {

	public GameObject explosionPrefab;
	private AudioSource audioSource;
	private GameObject currentPlanet;

	// Use this for initialization
	void Start () {
		audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision other) {
		CmdSpawnExplosion (other.gameObject);
		audioSource.Play();
		if (other.gameObject.tag == "Player") {
			Debug.Log ("Player hit!");
		}
		Debug.Log (other.gameObject.tag.ToString());
		Destroy(this);
	}

	[Command]
	private void CmdSpawnExplosion(GameObject other) {
		GameObject smoke = GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.LookRotation(transform.position - other.transform.position + new Vector3(0.0f, -90.0f, 0.0f)));
		NetworkServer.Spawn(smoke);
		Destroy (smoke, 5.0f);
	}

	//Mutator
	public GameObject CurrentPlanet {
		get {
			return currentPlanet;
		}
		set {
			currentPlanet = value;
		}
	}
}
