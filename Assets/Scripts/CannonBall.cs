using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CannonBall : NetworkBehaviour {

	[Tooltip("The explosion prefab instantiated when the cannonball collides with something.")] public GameObject explosionPrefab;
	[HideInInspector] public GameObject currentPlanet;
	private AudioSource audioSource;


	// Use this for initialization
	void Start () {
		audioSource = this.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Triggered when the cannonball collide with another object.
	/// </summary>
	private void OnCollisionEnter(Collision other) {
		if (hasAuthority) {
			CmdSpawnExplosion (other.gameObject);
		}
		audioSource.Play();
		if (other.gameObject.tag == "Player") {
			Debug.Log ("Player hit!");
		}
		Debug.Log (other.gameObject.tag.ToString());
		Destroy (this);
	}

	/// <summary>
	/// Spawns a GameObject that produce particle effect ressembling an explosion.
	/// </summary>
	[Command]
	private void CmdSpawnExplosion(GameObject other) {
		GameObject smoke = GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.LookRotation(transform.position - other.transform.position + new Vector3(0.0f, -90.0f, 0.0f)));
		NetworkServer.Spawn(smoke);
		Destroy (smoke, 5.0f);
	}
}
