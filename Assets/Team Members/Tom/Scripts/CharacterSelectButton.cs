using System.Collections;
using System.Collections.Generic;
using Gerallt;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectButton : NetworkBehaviour
{
    public int characterIndex;
    public UILobby uiLobby;
    public EventSystem eventSystem;
    
    public void SetCharacter()
    {
        // MASSIVE HACK - allows for character swapping after joining
        LobbyPlayerData? localPlayer = uiLobby.localPlayerData;
        LobbyPlayerData localClientPlayer = NetworkPlayerList.Instance.GetPlayerDataByClientId(NetworkManager.Singleton.LocalClientId);
        
        // Need to store as temporary variable since localPlayer is a nullable reference
        localPlayer = localClientPlayer;
        LobbyPlayerData temp = localPlayer.Value;
        temp.characterIndex = characterIndex;
        uiLobby.localPlayerData = temp;
        
        // Potentially a hack
        if (IsServer || IsClient)
        {
            if (uiLobby.localPlayerData.HasValue)
            {
                NetworkPlayerList.Instance.UpdatePlayerDataServerRpc(uiLobby.localPlayerData.Value);
            }
        }

        eventSystem.SetSelectedGameObject(gameObject);
    }
}