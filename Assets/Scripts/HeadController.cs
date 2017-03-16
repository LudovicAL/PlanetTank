using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class HeadController : NetworkBehaviour {

	[Tooltip("The turret left and right rotation speed.")] public float rotationSpeed;
	[Tooltip("The cannon maximum up or down rotation angle.")] public float maxAngle;
	[Tooltip("The time in seconds between each cannon shot.")] public float canonCoolDownDuration;
	[Tooltip("The force applied on a cannonball when it is shot.")] public float canonBallForce;
	[Tooltip("The lifespan of cannonballs in seconds.")] public float canonBallDuration;
	[Tooltip("The force of the recoil when the cannon is shot.")] public float recoilForce;
	[Tooltip("The lifespan of the smoke when the cannon is shot, in seconds.")] public float smokeDuration;
	[Tooltip("The smoke prefab instantiated when the cannon is shot.")] public GameObject ShootingFXPrefab;
	[Tooltip("The cannonball prefab instantiated when the cannon is shot.")] public GameObject canonBallPrefab;
	private GameObject head;
	private GameObject canonBase;
	private GameObject canonTip;
	private GameObject canon;
	private Rigidbody selfRigidbody;
	private AudioSource audioSource;
	private SmoothFollow cameraSmoothFollow;
	private GameObject scriptsBucket;
	private GameManager gameManager;
	private float remainingCoolDown = 0.0f;
	private ChatManager chatManager;
	private IdMaker idMaker;
	[SyncVar] public int health;
	NetworkClient client;
	const short CHAT_MSG = MsgType.Highest + 1;

	void Start () {
		client = GameObject.FindObjectOfType<NetworkManager>().client;
		if (client.isConnected) {
			client.RegisterHandler(CHAT_MSG, ClientReceiveChatMessage);
		}
		if (isServer) {
			NetworkServer.RegisterHandler(CHAT_MSG, ServerReceiveChatMessage);
		}
		head = transform.FindChild("Meshes").FindChild("Head").gameObject;
		canonBase = head.transform.FindChild("CanonBase").gameObject;
		canon = canonBase.transform.FindChild("Canon").gameObject;
		canonTip = canon.transform.FindChild("CanonTip").gameObject;
		selfRigidbody = this.GetComponent<Rigidbody>();
		cameraSmoothFollow = Camera.main.GetComponent<SmoothFollow> ();
		audioSource = canon.GetComponent<AudioSource>();
		scriptsBucket = GameObject.Find ("ScriptsBucket");
		gameManager = scriptsBucket.GetComponent<GameManager>();
		idMaker = this.GetComponent<IdMaker>();
		chatManager = GameObject.Find("Canvas").GetComponent<ChatManager> ();
		chatManager.headController = this.GetComponent<HeadController> ();
		chatManager.UpdateChatStatus (true);
		if (isLocalPlayer) {
			cameraSmoothFollow.target = head.transform;
			cameraSmoothFollow.UpdatePlanet ();
			UpdateCanonColor();
		}
	}

	void Update () {
		if (isLocalPlayer) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000.0f)) {
				RotateLeftRight (hit);
				RotateUpDown (hit);
			}
			if (Input.GetButtonDown("Fire1")) {
				FireCanon();
			}
			UpdateCannonCoolDown();
		}
	}

	public void LateUpdate() {
		if (isLocalPlayer) {
			cameraSmoothFollow.UpdateCameraPosition();
		}
	}

	/// <summary>
	/// Called when the GameObject is destroyed.
	/// </summary>
	void OnDestroy() {
		if (isLocalPlayer) {
			chatManager.UpdateChatStatus (false);
		}
	}

	/// <summary>
	/// Updates the cooldown timer used to determine the color of the cannon as it becomes red when shot.
	/// </summary>
	public void UpdateCannonCoolDown() {
		if (remainingCoolDown > 0.0f) {
			remainingCoolDown -= Time.fixedDeltaTime;
			remainingCoolDown = Mathf.Max(remainingCoolDown, 0.0f);
			UpdateCanonColor();
		}
	}

	/// <summary>
	/// Updates the cannon color: red if it just shot, default otherwise.
	/// </summary>
	public void UpdateCanonColor() {
		MeshRenderer meshRenderer = canon.GetComponent<MeshRenderer>();
		if (meshRenderer != null) {
			Color cannonColor = remainingCoolDown > 0.0f ? Color.red : Color.black;
			meshRenderer.material.color = cannonColor;
		}
	}

	/// <summary>
	/// Has the tank receive damage from a cannonball hit.
	/// </summary>
	public void TakeDamage() {
		if (!isServer) {
			health -= 1;
			if (health <= 0) {
				health = 0;
				Debug.Log ("Player died.");
				Destroy (this);
			}
		}
	}

	/// <summary>
	/// Fires a cannonball out of the cannon (client side).
	/// </summary>
	public void FireCanon() {
		if (remainingCoolDown <= 0.0f) {
			//Recoil
			selfRigidbody.AddForce(canon.transform.forward * -recoilForce);
			//Sound
			audioSource.Play();
			//Cooldown
			remainingCoolDown = canonCoolDownDuration;
			//Serverside stuff
			CmdFireCanon (); //Called from the client, but invoked on the server
		}
	}

	/// <summary>
	/// Fires a cannonball out of the cannon (server side).
	/// </summary>
	[Command]
	public void CmdFireCanon() { 			
		//Canonball
		GameObject cannonBall = GameObject.Instantiate(canonBallPrefab, canonTip.transform.position, Quaternion.identity, null);
		cannonBall.GetComponent<CannonBall> ().currentPlanet = gameManager.GetPlanet ();
		cannonBall.GetComponent<Rigidbody>().AddForce(canon.transform.forward * canonBallForce, ForceMode.Impulse);
		NetworkServer.Spawn(cannonBall);
		Destroy(cannonBall, canonBallDuration);
		//Particle Effect
		GameObject smoke = GameObject.Instantiate(ShootingFXPrefab, canonTip.transform.position, canon.transform.rotation, canonTip.transform);
		smoke.transform.localPosition = Vector3.zero;
		NetworkServer.Spawn(smoke);
		Destroy (smoke, smokeDuration);
	}

	/// <summary>
	/// Rotates the tank's turret left of right.
	/// </summary>
	public void RotateLeftRight(RaycastHit hit) {
		Vector3 direction = (head.transform.InverseTransformPoint (hit.point) - head.transform.localPosition);
		direction.y = 0.0f;
		direction.z = 0.0f;
		if (direction.magnitude > 0.1f) {
			Quaternion lookRotation = Quaternion.LookRotation (direction);
			head.transform.localRotation = Quaternion.RotateTowards (head.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
		}
	}
		
	/// <summary>
	/// Rotates the tank's cannon up or down.
	/// </summary>
	public void RotateUpDown(RaycastHit hit) {
		Vector3 direction = (canonBase.transform.InverseTransformPoint (hit.point) - canonBase.transform.localPosition);
		if (direction.y > 0.1f) {	//Raise the canon
			if (canonBase.transform.localEulerAngles.x < 180.0f || Vector3.Angle(head.transform.forward, canonBase.transform.forward) < maxAngle) {
				direction.x = 0.0f;
				direction.z = 0.0f;
				Quaternion lookRotation = Quaternion.LookRotation (direction);
				canonBase.transform.localRotation = Quaternion.RotateTowards (canonBase.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
			}
		} else if (direction.y < -0.1f) {	//Lower the canon
			if (canonBase.transform.localEulerAngles.x > 180.0f || Vector3.Angle(head.transform.forward, canonBase.transform.forward) < maxAngle) {
				direction.x = 0.0f;
				direction.z = 0.0f;
				Quaternion lookRotation = Quaternion.LookRotation (direction);
				canonBase.transform.localRotation = Quaternion.RotateTowards (canonBase.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
			}
		}
	}

	/// <summary>
	/// Sends a chat message.
	/// </summary>
	public void SendChatMessage (string msg) {
		StringMessage strMsg = new StringMessage(idMaker.playerUniqueId.ToString() + ": " + msg);
		if (isServer) {
			NetworkServer.SendToAll(CHAT_MSG, strMsg); // Send to all clients
		} else if (client.isConnected ) {
			client.Send(CHAT_MSG, strMsg); // Sending message from client to server
		}
	}

	/// <summary>
	/// Receives a chat message (server side).
	/// </summary>
	public void ServerReceiveChatMessage (NetworkMessage netMsg) {
		if (isServer) {
			string str = netMsg.ReadMessage<StringMessage>().value;
			StringMessage strMsg = new StringMessage(str);
			NetworkServer.SendToAll(CHAT_MSG, strMsg); // Send to all clients
		}
	}

	/// <summary>
	/// Receives a chat message (client side).
	/// </summary>
	public void ClientReceiveChatMessage (NetworkMessage netMsg) {
		if(client.isConnected) {
			string str = netMsg.ReadMessage<StringMessage>().value;
			int tempo;
			int.TryParse (str.Substring (0, str.IndexOf (":")), out tempo);
			bool ownMessage = (idMaker.playerUniqueId == tempo);
			chatManager.ReceiveChatMessage (ownMessage, "Player " + str);
		}
	}
}