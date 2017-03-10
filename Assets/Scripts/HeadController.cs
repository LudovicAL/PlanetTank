using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class HeadController : NetworkBehaviour {

	public float rotationSpeed;
	public float maxAngle;
	public float canonCoolDownDuration;
	public float canonBallForce;
	public float canonBallDuration;
	public float recoilForce;
	public float smokeDuration;
	public GameObject ShootingFXPrefab;
	public GameObject canonBallPrefab;
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
	[SyncVar] public int health;
	NetworkClient client;
	const short CHAT_MSG = MsgType.Highest + 1;

	// Use this for initialization
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
		chatManager = GameObject.Find("Canvas").GetComponent<ChatManager> ();
		chatManager.headController = this.GetComponent<HeadController> ();
		chatManager.UpdateChatStatus (true);
		if (isLocalPlayer) {
			cameraSmoothFollow.target = head.transform;
			cameraSmoothFollow.UpdatePlanet ();
			UpdateCanonColor();
		}
	}
	
	// Update is called once per frame
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

	void OnDestroy() {
		if (isLocalPlayer) {
			chatManager.UpdateChatStatus (false);
		}
	}

	public void LateUpdate() {
		if (isLocalPlayer) {
			cameraSmoothFollow.UpdateCameraPosition();
		}
	}

	//Updates the cannon cooldown timer
	public void UpdateCannonCoolDown() {
		if (remainingCoolDown > 0.0f) {
			remainingCoolDown -= Time.fixedDeltaTime;
			remainingCoolDown = Mathf.Max(remainingCoolDown, 0.0f);
			UpdateCanonColor();
		}
	}

	//Updates the cannon color: red if reloading, default if loaded
	public void UpdateCanonColor() {
		MeshRenderer meshRenderer = canon.GetComponent<MeshRenderer>();
		if (meshRenderer != null) {
			Color cannonColor = remainingCoolDown > 0.0f ? Color.red : Color.black;
			meshRenderer.material.color = cannonColor;
		}
	}

	//Fires a cannonball
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

	//Have the tank receive damage from a hit
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

	//Fires a cannonball (called from the client, but invoked on the server)
	[Command]
	public void CmdFireCanon() { 			
		//Canonball
		GameObject cannonBall = GameObject.Instantiate(canonBallPrefab, canonTip.transform.position, Quaternion.identity, null);
		cannonBall.GetComponent<CannonBall> ().CurrentPlanet = gameManager.GetPlanet ();
		cannonBall.GetComponent<Rigidbody>().AddForce(canon.transform.forward * canonBallForce, ForceMode.Impulse);
		NetworkServer.Spawn(cannonBall);
		Destroy(cannonBall, canonBallDuration);
		//Particle Effect
		GameObject smoke = GameObject.Instantiate(ShootingFXPrefab, canonTip.transform.position, canon.transform.rotation, canonTip.transform);
		smoke.transform.localPosition = Vector3.zero;
		NetworkServer.Spawn(smoke);
		Destroy (smoke, smokeDuration);
	}

	//Rotates the turret left or right
	public void RotateLeftRight(RaycastHit hit) {
		Vector3 direction = (head.transform.InverseTransformPoint (hit.point) - head.transform.localPosition);
		direction.y = 0.0f;
		direction.z = 0.0f;
		if (direction.magnitude > 0.1f) {
			Quaternion lookRotation = Quaternion.LookRotation (direction);
			head.transform.localRotation = Quaternion.RotateTowards (head.transform.localRotation, lookRotation, rotationSpeed * Time.deltaTime);
		}
	}
		
	//Rotates the cannon up or down
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

	//Sends a chat message
	public void SendChatMessage (string msg) {
		StringMessage strMsg = new StringMessage(GetClientId() + ":" + msg);
		if (isServer) {
			NetworkServer.SendToAll(CHAT_MSG, strMsg); // Send to all clients
		} else if (client.isConnected ) {
			client.Send(CHAT_MSG, strMsg); // Sending message from client to server
		}
	}

	//Server receives chat message
	public void ServerReceiveChatMessage (NetworkMessage netMsg) {
		if (isServer) {
			string str = netMsg.ReadMessage<StringMessage>().value;
			StringMessage strMsg = new StringMessage(str);
			NetworkServer.SendToAll(CHAT_MSG, strMsg); // Send to all clients
		}
	}

	//Client receives chat message
	public void ClientReceiveChatMessage (NetworkMessage netMsg) {
		if(client.isConnected) {
			string str = netMsg.ReadMessage<StringMessage>().value;
			int tempo;
			int.TryParse (str.Substring (0, str.IndexOf (":")), out tempo);
			bool ownMessage = (GetClientId() == tempo);
			chatManager.ReceiveChatMessage (ownMessage, str); // Add the message to the client's local chat window
		}
	}
		
	//Returns the client connection id
	public int GetClientId() {
		int result;
		int.TryParse(this.GetComponent<NetworkIdentity> ().netId.Value.ToString (), out result);
		return result;
	}
}