using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatusManager : MonoBehaviour {

	[HideInInspector]
	public GameObject localPlayer;
	private Text text;
	private GameManager gameManager;

	void Start () {
		text = this.gameObject.GetComponent<Text> ();
		gameManager = GameObject.Find ("ScriptsBucket").GetComponent<GameManager>();
		UpdateStatus ();
	}
	
	/// <summary>
	/// Updates the game status displayed in the top right corner of the screen during gameplay
	/// </summary>
	public void UpdateStatus () {
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
