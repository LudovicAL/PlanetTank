using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer (NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer> ();
		HeadController headController = gamePlayer.GetComponent<HeadController> ();
		headController.pName = lobby.playerName;
		headController.pColor = lobby.playerColor;
	}
}
