using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

	[Tooltip("The message prefab instantiated when a new message is added to the conversation.")] public GameObject chatMessagePrefab;
	public Text toggleButtonText;
	public Button sendButton;
	public GameObject PanelMessages;
	public InputField inputField;
	private Animator chatPanelAnimator;
	private List<GameObject> messagesList;
	[HideInInspector] public HeadController headController;

	void Start () {
		messagesList = new List<GameObject> ();
		chatPanelAnimator = this.GetComponent<Animator> ();
		UpdateChatToggleButtonText ();
	}

	/// <summary>
	/// Shows or hides the chat Panel.
	/// </summary>
	public void ToggleChatPanel() {
		chatPanelAnimator.SetBool ("Show", !chatPanelAnimator.GetBool ("Show"));
		UpdateChatToggleButtonText ();
	}

	/// <summary>
	/// Updates the chat Panel's header in order for it to display either 'show' or 'hide' according to the situation.
	/// </summary>
	private void UpdateChatToggleButtonText() {
		if (chatPanelAnimator.GetBool ("Show")) {
			toggleButtonText.text = "Hide";
		} else {
			toggleButtonText.text = "Show";
		}
	}

	/// <summary>
	/// Prevent the user from writing a message longer then a specified number of characters.
	/// </summary>
	public void LimitChatMessageLength() {
		if (inputField.text.Length > 40) {
			inputField.text = inputField.text.Substring (0, 39);
		}
	}

	/// <summary>
	/// Erase the message written by the user that is not sent yet.
	/// </summary>
	public void CancelChatMessage() {
		inputField.text = "";
	}

	/// <summary>
	/// Sends a chat message.
	/// </summary>
	public void SendChatMessage() {
		if (inputField.text.Length > 0) {
			headController.SendChatMessage (inputField.text);
			inputField.text = "";
			inputField.ActivateInputField ();
			inputField.Select ();
		}
	}

	/// <summary>
	/// Receives a chat message.
	/// </summary>
	public void ReceiveChatMessage(string content) {
		if (content != null && content.Length > 0) {
			//Analyzes the message content
			string[] contents = content.Split(new[] {"::"}, System.StringSplitOptions.None);
			if (contents != null && contents.Length == 6) {
				float colorR = float.Parse(contents[0]);
				float colorG = float.Parse(contents[1]);
				float colorB = float.Parse(contents[2]);
				float colorA = float.Parse(contents[3]);
				Color messageColor = new Color (colorR, colorG, colorB, colorA);
				//Instantiates the new message gameobject
				GameObject messageGo = GameObject.Instantiate(chatMessagePrefab, PanelMessages.transform);
				messageGo.transform.localScale = Vector3.one;
				//Inserts the player name
				Text t = messageGo.transform.FindChild ("Text Name").GetComponent<Text>();
				t.text = contents[4] + ":";
				t.color = messageColor;
				//Inserts the message content
				t = messageGo.transform.FindChild ("Text Content").GetComponent<Text>();
				t.text = contents[5];
				//Deletes older messages if they overflow
				messagesList.Add (messageGo);
				if (messagesList.Count > 5) {
					Destroy(messagesList [0]);
					messagesList.RemoveAt (0);
				}
			}
		}
	}
}
