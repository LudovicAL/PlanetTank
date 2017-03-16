using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CanvasManager : MonoBehaviour {

	[Tooltip("Reference here all panels in the Canvas.")] public GameObject[] Panels;
	[Tooltip("Reference here the InputField Port.")] public GameObject inputFieldPort;
	[Tooltip("Reference here the InputField IpAddress.")] public GameObject inputFieldIpAddress;

	void Start() {
		ActivatePanel ("Panel Menu");
	}

	/// <summary>
	/// Activates the specified Panel and desactivate all other Panels.
	/// </summary>
	private void ActivatePanel(string panelName) {
		foreach (GameObject panel in Panels) {
			if (panel.transform.name == panelName) {
				panel.SetActive (true);
			} else {
				panel.SetActive (false);
			}
		}
	}

	/// <summary>
	/// Starts a new game as host.
	/// </summary>
	public void StartGameAsHost() {
		try {
			SetPort ();
			CustomNetworkManager.singleton.StartHost ();
		} catch (UserInputException e) {
			Console.WriteLine("UserInputIsWrongException: ", e.Message);
		}
	}

	/// <summary>
	/// Joins an existing game as client
	/// </summary>
	public void JoinGameAsClient() {
		try {
			SetPort();
			CustomNetworkManager.singleton.StartClient ();
		} catch (UserInputException e) {
			Console.WriteLine("UserInputIsWrongException: ", e.Message);
		}
	}

	public void StartLanServerOnlyGame() {
		CustomNetworkManager.singleton.StartServer ();
	}

	/// <summary>
	/// Starts the MatchMaker.
	/// </summary>
	public void StartMatchMaker() {
		CustomNetworkManager.singleton.StartMatchMaker ();
	}

	/// <summary>
	/// Resume the game when game is on pause.
	/// </summary>
	public void Resume() {
		
	}

	/// <summary>
	/// Stops both the client and server that the Network Manager is running.
	/// </summary>
	public void Disconnect() {
		CustomNetworkManager.singleton.StopHost ();
	}

	/// <summary>
	/// Quits the application.
	/// </summary>
	public void Quit() {
		Application.Quit ();
	}

	/// <summary>
	/// Sets the NetworkPort value.
	/// </summary>
	private void SetPort() {
		int networkPort;
		if (!int.TryParse (inputFieldPort.GetComponent<InputField>().text, out networkPort)){
			throw (new UserInputException("Unable to convert the user input."));
		}
		CustomNetworkManager.singleton.networkPort = networkPort;
	}

	/// <summary>
	/// Sets the NetworkAddress value.
	/// </summary>
	private void SetIpAdress() {
		string ipAddress = inputFieldIpAddress.GetComponent<InputField>().text;
		if (ipAddress.Length == 0) {
			throw (new UserInputException("The user left the IP address input field blank."));
		}
		CustomNetworkManager.singleton.networkAddress = ipAddress;
	}
}

public class UserInputException: Exception {
	public UserInputException(string message): base(message) {
	}
}