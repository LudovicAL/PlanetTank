using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatusWatcher : MonoBehaviour {

	[HideInInspector]
	public GameObject localPlayer;
	private Text text;
	private GameManager gameManager;

	void Awake () {
		text = this.gameObject.GetComponent<Text> ();
		GetGameManager ();
	}

	/// <summary>
	/// Fetches a reference to the GameManager if that reference was not acquired yet.
	/// </summary>
	private void GetGameManager() {
		if (gameManager == null) {
			GameObject scriptsBucket = GameObject.Find ("ScriptsBucket");
			if (scriptsBucket != null) {
				gameManager = scriptsBucket.GetComponent<GameManager> ();
			}
		}
	}

	/// <summary>
	/// Updates the game status displayed in the top right corner of the screen during gameplay
	/// </summary>
	public void UpdateStatus () {
		GetGameManager ();
		if (gameManager != null && text != null) {
			int count = 0;
			foreach (GameObject go in gameManager.tankList) {
				if (go != localPlayer) {
					count++;
				}
			}
			if (count > 1) {
				text.text = count.ToString () + " enemies left";
			} else {
				text.text = count.ToString () + " enemy left";
			}
		}
	}
}
