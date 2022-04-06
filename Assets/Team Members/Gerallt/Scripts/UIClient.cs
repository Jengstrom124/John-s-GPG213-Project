using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gerallt
{
    public class UIClient : MonoBehaviour
    {
        public TextMeshProUGUI UITextMeshPro;
        public UILobby parentView;

        public void UpdateUI(LobbyPlayerData lobbyPlayerData)
        {
            UITextMeshPro.SetText(lobbyPlayerData.ClientId.ToString() 
                                  + " - " + lobbyPlayerData.PlayerName
                                  + " - " + lobbyPlayerData.ClientIPAddress);
        }

        // Start is called before the first frame update
        void Start()
        {
            parentView.OnPlayerDataChanged += ParentView_OnPlayerDataChanged;
        }

        private void ParentView_OnPlayerDataChanged(LobbyPlayerData newPlayerData)
        {
            UpdateUI(newPlayerData);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
