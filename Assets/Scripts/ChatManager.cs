using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

	[Tooltip("The message prefab instantiated when a new message is added to the conversation.")] public GameObject chatMessagePrefab;
	public GameObject chatPanel;
	public Text toggleButtonText;
	public Button sendButton;
	public GameObject PanelMessages;
	public InputField inputField;
	private Animator chatPanelAnimator;
	private List<GameObject> messagesList;
	[HideInInspector] public HeadController headController;

	void Start () {
		messagesList = new List<GameObject> ();
		chatPanel = GameObject.Find ("Panel Chat");
		chatPanelAnimator = chatPanel.GetComponent<Animator> ();
		UpdateChatToggleButtonText ();
		UpdateChatStatus (false);
	}

	/// <summary>
	/// Activates or desactivates the chat Panel.
	/// </summary>
	public void UpdateChatStatus(bool active) {
		if (chatPanel != null) {
			chatPanel.SetActive (active);
		}
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
	/// Receives a chat message.
	/// </summary>
	public void ReceiveChatMessage(bool ownMessage, string content) {
		GameObject messageGo = GameObject.Instantiate(chatMessagePrefab, PanelMessages.transform);
		messageGo.transform.localScale = Vector3.one;
		Text t = messageGo.transform.FindChild ("Text Name").GetComponent<Text>();
		t.text = content.Substring (0, content.IndexOf (":") + 1);
		t.color = (ownMessage) ? Color.red : Color.blue;
		t = messageGo.transform.FindChild ("Text Content").GetComponent<Text>();
		t.text = content.Substring (content.IndexOf (":") + 1);
		messagesList.Add (messageGo);
		if (messagesList.Count > 5) {
			Destroy(messagesList [0]);
			messagesList.RemoveAt (0);
		}
	}
}
