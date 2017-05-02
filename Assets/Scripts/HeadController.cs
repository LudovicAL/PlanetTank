using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class HeadController : NetworkBehaviour {

	[Header("Head rotation")]
	[Tooltip("The turret left and right rotation speed.")] public float rotationSpeed;
	[Tooltip("The cannon maximum up or down rotation angle.")] public float maxAngle;
	[Header("Cannon")]
	[Tooltip("The time in seconds between each cannon shot.")] public float cannonCoolDownDuration;
	[Tooltip("The force of the recoil when the cannon is shot.")] public float recoilForce;
	[Header("Cannonballs")]
	[Tooltip("The cannonball prefab instantiated when the cannon is shot.")] public GameObject cannonBallPrefab;
	[Tooltip("The force applied on a cannonball when it is shot.")] public float cannonBallForce;
	[Tooltip("The lifespan of cannonballs in seconds.")] public float cannonBallDuration;
	[Header("Particle Effects")]
	[Tooltip("The lifespan of the smoke when the cannon is shot, in seconds.")] public float smokeDuration;
	[Tooltip("The smoke prefab instantiated when the cannon is shot.")] public GameObject ShootingFXPrefab;
	[Header("Health system")]
	[Tooltip("Maximum hit points")] public int maxHealth;
	[SyncVar] public int currentHealth;
	[Header("Player infos")]
	[SyncVar] public string pName;
	[SyncVar] public Color pColor;
	private GameObject head;
	private GameObject cannonBase;
	private GameObject cannonTip;
	private GameObject cannon;
	private Rigidbody selfRigidbody;
	private AudioSource audioSource;
	private GameObject scriptsBucket;
	private GameManager gameManager;
	private float remainingCoolDown = 0.0f;
	private ChatManager chatManager;
	private Vector3 spawnPosition;
	private NetworkClient client;
	private HealthBarManager healthBarManager;
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
		cannonBase = head.transform.FindChild("CannonBase").gameObject;
		cannon = cannonBase.transform.FindChild("Cannon").gameObject;
		cannonTip = cannon.transform.FindChild("CannonTip").gameObject;
		selfRigidbody = this.GetComponent<Rigidbody>();
		audioSource = cannon.GetComponent<AudioSource>();
		scriptsBucket = GameObject.Find ("ScriptsBucket");
		gameManager = scriptsBucket.GetComponent<GameManager>();
		chatManager = GameObject.Find("LobbyManager").transform.FindChild("ChatPanel").GetComponent<ChatManager>();
		GameObject flag = this.transform.FindChild ("Flag").gameObject;
		flag.GetComponent<TextMesh> ().text = pName;
		flag.GetComponent<TextMesh> ().color = pColor;
		if (isLocalPlayer) {
			healthBarManager = GameObject.Find ("HealthPanel").GetComponent<HealthBarManager> ();
			healthBarManager.UpdateHealthBar (currentHealth, maxHealth);
			flag.SetActive(false);
			chatManager.headController = this;
			CmdGoToSpawn ();
			Camera.main.GetComponent<SmoothFollow> ().target = head.transform;
			UpdatecannonColor();
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
				Firecannon();
			}
			UpdateCannonCoolDown();
			if (Input.GetButtonDown("Jump")) {
				MoveToPosition (spawnPosition);
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Command]
	public void CmdGoToSpawn() {
		gameManager.MovePlayerToHisSpawn (this.gameObject);
	}

	/// <summary>
	/// Sets the tank spawn position and rotation and calls the MoveToPosition function with these parameters
	/// </summary>
	[ClientRpc]
	public void RpcSetSpawn(Vector3 position) {
		spawnPosition = position;
		MoveToPosition (position);
	}

	/// <summary>
	/// Moves the tank to a given position
	/// </summary>
	public void MoveToPosition(Vector3 position) {
		if (isLocalPlayer) {
			gameObject.transform.position = position;
			gameObject.transform.LookAt(gameManager.GetPlanet ().transform.position);
			gameObject.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
		}
	}

	/// <summary>
	/// Updates the cooldown timer used to determine the color of the cannon as it becomes red when shot.
	/// </summary>
	public void UpdateCannonCoolDown() {
		if (remainingCoolDown > 0.0f) {
			remainingCoolDown -= Time.fixedDeltaTime;
			remainingCoolDown = Mathf.Max(remainingCoolDown, 0.0f);
			UpdatecannonColor();
		}
	}

	/// <summary>
	/// Updates the cannon color: red if it just shot, default otherwise.
	/// </summary>
	public void UpdatecannonColor() {
		MeshRenderer meshRenderer = cannon.GetComponent<MeshRenderer>();
		if (meshRenderer != null) {
			Color cannonColor = remainingCoolDown > 0.0f ? Color.red : Color.black;
			meshRenderer.material.color = cannonColor;
		}
	}

	/// <summary>
	/// Checks for localPlayer before calling the CmdTakeDamge function
	/// </summary>
	public void TakeDamage() {
		if (isLocalPlayer) {
			CmdTakeDamage ();
		}
	}

	/// <summary>
	/// Has the tank receive damage from a cannonball hit.
	/// </summary>
	[Command]
	private void CmdTakeDamage() {
		currentHealth--;
		RpcAssessDamage ();
	}

	/// <summary>
	/// Assess the current status of the tank after it received damage.
	/// </summary>
	[ClientRpc]
	private void RpcAssessDamage() {
		if (isLocalPlayer) {
			healthBarManager.UpdateHealthBar (currentHealth, maxHealth);
		}
		if (currentHealth <= 0) {
			Die ();
		}
	}

	private void Detach(GameObject go){
		go.transform.parent = null;
		go.AddComponent<Gravity> ();
		go.AddComponent<Rigidbody> ();
		go.GetComponent<Rigidbody> ().useGravity = false;
		go.GetComponent<Rigidbody> ().mass = 300;
	}

	/// <summary>
	/// Kills the tank.
	/// </summary>
	private void Die() {
		Detach (cannon);
		Detach (head);
		Detach (this.transform.Find ("Meshes").Find ("Body").gameObject);
		GetComponent<UnityStandardAssets.Vehicles.Car.CarController> ().enabled = false;
		GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl> ().enabled = false;
		GetComponent<UnityStandardAssets.Vehicles.Car.CarAudio> ().enabled = false;
		GetComponent<HeadController> ().enabled = false;
	}

	/// <summary>
	/// Fires a cannonball out of the cannon (client side).
	/// </summary>
	public void Firecannon() {
		if (remainingCoolDown <= 0.0f) {
			//Recoil
			selfRigidbody.AddForce(cannon.transform.forward * -recoilForce);
			//Sound
			audioSource.Play();
			//Cooldown
			remainingCoolDown = cannonCoolDownDuration;
			//Serverside stuff
			CmdFireCannon (); //Called from the client, but invoked on the server
		}
	}

	/// <summary>
	/// Fires a cannonball out of the cannon (server side).
	/// </summary>
	[Command]
	public void CmdFireCannon() { 			
		//Cannonball
		GameObject cannonBall = GameObject.Instantiate(cannonBallPrefab, cannonTip.transform.position, Quaternion.identity, null);
		cannonBall.GetComponent<CannonBall> ().currentPlanet = gameManager.GetPlanet ();
		cannonBall.GetComponent<Rigidbody>().AddForce(cannon.transform.forward * cannonBallForce, ForceMode.Impulse);
		NetworkServer.Spawn(cannonBall);
		Destroy(cannonBall, cannonBallDuration);
		//Particle Effect
		GameObject smoke = GameObject.Instantiate(ShootingFXPrefab, cannonTip.transform.position, cannon.transform.rotation, cannonTip.transform);
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
		Vector3 direction = (cannonBase.transform.InverseTransformPoint (hit.point) - cannonBase.transform.localPosition);
		if (direction.y > 0.1f) {	//Raise the cannon
			if (cannonBase.transform.localEulerAngles.x < 180.0f || Vector3.Angle(head.transform.forward, cannonBase.transform.forward) < maxAngle) {
				direction.x = 0.0f;
				direction.z = 0.0f;
				Quaternion lookRotation = Quaternion.LookRotation (direction);
				cannonBase.transform.localRotation = Quaternion.RotateTowards (cannonBase.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
			}
		} else if (direction.y < -0.1f) {	//Lower the cannon
			if (cannonBase.transform.localEulerAngles.x > 180.0f || Vector3.Angle(head.transform.forward, cannonBase.transform.forward) < maxAngle) {
				direction.x = 0.0f;
				direction.z = 0.0f;
				Quaternion lookRotation = Quaternion.LookRotation (direction);
				cannonBase.transform.localRotation = Quaternion.RotateTowards (cannonBase.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
			}
		}
	}

	/// <summary>
	/// Sends a chat message.
	/// </summary>
	public void SendChatMessage (string msg) {
		if (msg != null && msg.Length > 0) {
			StringMessage strMsg = new StringMessage(
				pColor.r.ToString() + "::"
				+ pColor.g.ToString() + "::"
				+ pColor.b.ToString() + "::"
				+ pColor.a.ToString() + "::"
				+ pName + "::"
				+ msg
			);
			if (isServer) {
				NetworkServer.SendToAll(CHAT_MSG, strMsg); // Send to all clients
			} else if (client.isConnected ) {
				client.Send(CHAT_MSG, strMsg); // Sending message from client to server
			}
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
			chatManager.ReceiveChatMessage (str);
		}
	}
}