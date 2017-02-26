using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {

	public GameObject chatMessagePrefab;
	public GameObject chatPanel;
	public Text toggleButtonText;
	public Button sendButton;
	public GameObject PanelMessages;
	public InputField inputField;
	private Animator chatPanelAnimator;
	private List<GameObject> messagesList;
	[HideInInspector] public HeadController headController;

	// Use this for initialization
	void Start () {
		messagesList = new List<GameObject> ();
		chatPanel = GameObject.Find ("Panel Chat");
		chatPanelAnimator = chatPanel.GetComponent<Animator> ();
		UpdateChatToggleButtonText ();
		UpdateChatStatus (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateChatStatus(bool active) {
		if (chatPanel != null) {
			chatPanel.SetActive (active);
		}
	}

	public void SendChatMessage() {
		if (inputField.text.Length > 0) {
			headController.SendChatMessage (inputField.text);
			inputField.text = "";
			inputField.ActivateInputField ();
			inputField.Select ();
		}
	}

	public void ReceiveChatMessage(bool localPlayer, int id, string content) {
		GameObject messageGo = GameObject.Instantiate(chatMessagePrefab, PanelMessages.transform);
		Text t = (Text)messageGo.transform.FindChild ("Text Name").GetComponent<Text>();
		t.text = "Player " + id.ToString() + ": ";
		t.color = (localPlayer) ? Color.red : Color.blue;
		t = messageGo.transform.FindChild ("Text Content").GetComponent<Text>();
		t.text = content;
		messagesList.Add (messageGo);
		if (messagesList.Count > 5) {
			Destroy(messagesList [0]);
			messagesList.RemoveAt (0);
		}
	}

	public void ToggleChatPanel() {
		chatPanelAnimator.SetBool ("Show", !chatPanelAnimator.GetBool ("Show"));
		UpdateChatToggleButtonText ();
	}

	private void UpdateChatToggleButtonText() {
		if (chatPanelAnimator.GetBool ("Show")) {
			toggleButtonText.text = "Hide";
		} else {
			toggleButtonText.text = "Show";
		}
	}

	public void LimitChatMessageLength() {
		if (inputField.text.Length > 40) {
			inputField.text = inputField.text.Substring (0, 39);
		}
	}

	public void CancelChatMessage() {
		inputField.text = "";
	}
}
