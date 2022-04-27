using System.Collections;
using System.Collections.Generic;
using Gerallt;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    public GameManager.CharacterTypes characterType;
    public UILobby uiLobby;
    public EventSystem eventSystem;
    
    public void SetCharacter()
    {
        //NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().selectedCharacter = character;
        LobbyPlayerData? localPlayer = uiLobby.localPlayerData;

        // Potentially a hack
        if (localPlayer == null)
        {
            localPlayer = NetworkPlayerList.Instance.GetPlayerDataByClientId(NetworkManager.Singleton.LocalClientId);
            uiLobby.localPlayerData = localPlayer;
        }

        LobbyPlayerData temp = localPlayer.Value;
        temp.characterIndex = (int)characterType;
        localPlayer = temp;
        
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
