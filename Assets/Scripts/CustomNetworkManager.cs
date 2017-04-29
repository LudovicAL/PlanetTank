using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

	///////////////////////////////////////
	///Functions invoked on the Server/Host:
	///////////////////////////////////////

	/// <summary>
	/// Called when a client connects 
	/// </summary>
	override public void OnServerConnect(NetworkConnection conn) {
		Debug.Log ("Server Connected.");
	}

	/// <summary>
	/// Called when a client disconnects
	/// </summary>
	override public void OnServerDisconnect(NetworkConnection conn) {
		base.OnServerDisconnect(conn);
		Debug.Log ("Server disconnected.");
	}

	/// <summary>
	/// Called when a client is ready
	/// </summary>
	override public void OnServerReady(NetworkConnection conn) {
		base.OnServerReady (conn);
		Debug.Log ("Server ready.");
		if (NetworkServer.connections.Count <= 1) {
			//GameObject.Find ("ScriptsBucket").GetComponent<GameManager> ().RandomizePlanet ();
		}
	}

	/// <summary>
	/// Called when a new player is added for a client
	/// </summary>
	override public void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		base.OnServerAddPlayer (conn, playerControllerId);
		Debug.Log ("Server added player.");
	}

	/// <summary>
	/// Called when a player is removed for a client
	/// </summary>
	override public void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController) {
		base.OnServerRemovePlayer (conn, playerController);
		Debug.Log ("Server removed player.");
	}

	/// <summary>
	/// Called when a network error occurs
	/// </summary>
	override public void OnServerError(NetworkConnection conn, int errorCode) {
		Debug.Log ("Server error.");
	}

	///////////////////////////////////////
	///Functions invoked on the client:
	///////////////////////////////////////

	/// <summary>
	/// Called when connected to a server
	/// </summary>
	override public void OnClientConnect(NetworkConnection conn) {
		base.OnClientConnect (conn);
		Debug.Log ("Client connected.");
	}

	/// <summary>
	/// Called when disconnected from a server
	/// </summary>
	override public void OnClientDisconnect(NetworkConnection conn) {
		base.OnClientDisconnect (conn);
		Debug.Log ("Client disconnected.");
	}

	/// <summary>
	/// Called when a network error occurs
	/// </summary>
	override public void OnClientError(NetworkConnection conn, int errorCode) {
		Debug.Log ("Client error.");
	}

	/// <summary>
	/// Called when told to be not-ready by a server
	/// </summary>
	public override void OnClientNotReady(NetworkConnection conn) {
		Debug.Log ("Client not ready.");
	}

	/// <summary>
	/// Called when a match is created
	/// </summary>
	public override void OnMatchCreate(bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo matchInfo) {
		base.OnMatchCreate (success, extendedInfo, matchInfo);
		Debug.Log ("Match created.");
	}

	/// <summary>
	/// Called when a list of matches is received
	/// </summary>
	public override void OnMatchList(bool success, string extendedInfo, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matchList) {
		base.OnMatchList (success, extendedInfo, matchList);
		Debug.Log ("Match list received.");
	}

	///////////////////////////////////////
	///
	///////////////////////////////////////

	/// <summary>
	/// Called when a match is joined
	/// </summary>
	public override void OnMatchJoined(bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo matchInfo) {
		base.OnMatchJoined (success, extendedInfo, matchInfo);
		Debug.Log ("Match joined.");
	}

	public override void OnStartHost() {
		base.OnStartHost ();
		Debug.Log ("OnStartHost()");
	}

	public override void OnStartServer() {
		base.OnStartServer ();
		Debug.Log ("OnStartServer()");
	}
}
