using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IdMaker : NetworkBehaviour {

	[SyncVar] public string playerUniqueId;
	private NetworkInstanceId playerNetId;

	override public void OnStartLocalPlayer() {
		GetNetId();
	}

	[Client]
	public void GetNetId() {
		playerNetId = GetComponent<NetworkIdentity> ().netId;
		CmdTellServerMyId (MakeUniqueId ());
	}

	[Command]
	public void CmdTellServerMyId(string id) {
		playerUniqueId = id;
	}

	public string MakeUniqueId() {
		return "Player " + playerNetId.ToString();
	}
}
