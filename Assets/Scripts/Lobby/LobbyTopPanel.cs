﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby {
    public class LobbyTopPanel : MonoBehaviour {
		public GameObject panelGameStatus;
		public GameObject panelChat;
		public GameObject panelHealth;
        public bool isInGame = false;
        protected bool isDisplayed = true;
        protected Image panelImage;
        
		void Start() {
            panelImage = GetComponent<Image>();
        }

        void Update() {
			if (!isInGame) {
                return;
			}
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ToggleVisibility(!isDisplayed);
            }
        }

        public void ToggleVisibility(bool visible) {
            isDisplayed = visible;
			if (panelGameStatus != null) {
				panelGameStatus.SetActive (!visible);
			}
			if (panelChat != null) {
				panelChat.SetActive (!visible);
			}
			if (panelHealth != null) {
				panelHealth.SetActive (!visible);
			}
            foreach (Transform t in transform) {
                t.gameObject.SetActive(isDisplayed);
            }
            if (panelImage != null) {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}