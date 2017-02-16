using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

	///////////////////////////////////////
	///Functions invoked on the Server/Host:
	///////////////////////////////////////

	//Called when a client connects 
	override public void OnServerConnect(NetworkConnection conn) {
		Debug.Log ("Server Connected.");
	}

	//Called when a client disconnects
	override public void OnServerDisconnect(NetworkConnection conn) {
		base.OnServerDisconnect(conn);
		Debug.Log ("Server disconnected.");
	}

	//Called when a client is ready
	override public void OnServerReady(NetworkConnection conn) {
		base.OnServerReady (conn);
		Debug.Log ("Server ready.");
		GameObject.Find ("ScriptsBucket").GetComponent<GameManager> ().RandomizePlanet ();
	}

	//Called when a new player is added for a client
	override public void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
		base.OnServerAddPlayer (conn, playerControllerId);
		Debug.Log ("Server added player.");
	}

	//Called when a player is removed for a client
	override public void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController) {
		base.OnServerRemovePlayer (conn, playerController);
		Debug.Log ("Server removed player.");
	}

	//Called when a network error occurs
	override public void OnServerError(NetworkConnection conn, int errorCode) {
		Debug.Log ("Server error.");
	}

	///////////////////////////////////////
	///Functions invoked on the client:
	///////////////////////////////////////

	//Called when connected to a server
	override public void OnClientConnect(NetworkConnection conn) {
		base.OnClientConnect (conn);
		Debug.Log ("Client connected.");
	}

	//Called when disconnected from a server
	override public void OnClientDisconnect(NetworkConnection conn) {
		base.OnClientDisconnect (conn);
		Debug.Log ("Client disconnected.");
	}

	//Called when a network error occurs
	override public void OnClientError(NetworkConnection conn, int errorCode) {
		Debug.Log ("Client error.");
	}

	//Called when told to be not-ready by a server
	public override void OnClientNotReady(NetworkConnection conn) {
		Debug.Log ("Client not ready.");
	}

	//Called when a match is created
	public override void OnMatchCreate(bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo matchInfo) {
		base.OnMatchCreate (success, extendedInfo, matchInfo);
		Debug.Log ("Match created.");
	}

	//Called when a list of matches is received
	public override void OnMatchList(bool success, string extendedInfo, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matchList) {
		base.OnMatchList (success, extendedInfo, matchList);
		Debug.Log ("Match list received.");
	}

	///////////////////////////////////////
	///
	///////////////////////////////////////

	//Called when a match is joined
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
