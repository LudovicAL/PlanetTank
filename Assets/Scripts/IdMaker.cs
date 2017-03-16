using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IdMaker : NetworkBehaviour {

	[SyncVar] public int playerUniqueId;

	override public void OnStartLocalPlayer() {
		ClientCommunicateId();
	}

	/// <summary>
	/// Communicate the client id to the server (client side).
	/// </summary>
	[Client]
	public void ClientCommunicateId() {
		CmdServerBroadcastMyId (MakeUniqueId ());
	}

	/// <summary>
	/// Broadcast the id received from a client to all clients (server side).
	/// </summary>
	[Command]
	public void CmdServerBroadcastMyId(int id) {
		playerUniqueId = id;
	}

	/// <summary>
	/// Constructs a playerUniqueId.
	/// </summary>
	public int MakeUniqueId() {
		NetworkInstanceId playerNetId = GetComponent<NetworkIdentity> ().netId;
		return int.Parse(playerNetId.ToString());
	}
}
