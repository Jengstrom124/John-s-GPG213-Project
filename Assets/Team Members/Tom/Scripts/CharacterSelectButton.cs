using System.Collections;
using System.Collections.Generic;
using Gerallt;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    public int characterIndex;
    public UILobby uiLobby;
    public EventSystem eventSystem;
    
    public void SetCharacter()
    {
        LobbyPlayerData? localPlayer = uiLobby.localPlayerData;

        // Potentially a hack
        if (localPlayer == null)
        {
            localPlayer = NetworkPlayerList.Instance.GetPlayerDataByClientId(NetworkManager.Singleton.LocalClientId);
            uiLobby.localPlayerData = localPlayer;
        }

        // Need to store as temporary variable since localPlayer is a nullable reference
        LobbyPlayerData temp = localPlayer.Value;
        temp.characterIndex = characterIndex;
        uiLobby.localPlayerData = temp;
        
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
