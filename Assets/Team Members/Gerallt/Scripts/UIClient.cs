using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
    }
}
