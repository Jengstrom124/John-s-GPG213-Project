using System;
using System.Collections;
using System.Collections.Generic;
using Tom;
using Unity.Netcode;
using UnityEngine;
using Gerallt;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : ManagerBase<GameManager>
{
    public List<UnityEngine.Object> levels = new List<UnityEngine.Object>();
    public NetworkVariable<NetworkableString> selectedLevel = new NetworkVariable<NetworkableString>(string.Empty);

    public NetworkPlayerList networkList;

    public CameraFollow camera;
    public event Action StartedGameEvent;

    public event Action<bool> OnChangeLobbyVisibility;
    public event Action<bool> OnChangeLevelsVisibility;

    public GameObject playerStatsUIPrefab;
    

    // public void Start()
    // {
    //     NetworkManager.Singleton.OnClientConnectedCallback += SetupPlayer;
    // }
    //
    // public void SetupPlayer(ulong clientID)
    // {
    //     if (NetworkManager.Singleton.IsServer)
    //     {
    //         if (!GetComponent<NetworkObject>().IsSpawned)
    //         {
    //             GetComponent<NetworkObject>().Spawn();
    //         }
    //
    //         NetworkObjectReference playerObject = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
    //         SetupCameraClientRpc(clientID, playerObject);
    //     }
    // }

    [ClientRpc]
    public void SetupCameraClientRpc(ulong clientID, NetworkObjectReference playerObjectRef)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            GameObject playerObject = playerObjectRef;
            camera.target = playerObject.transform;
            camera.offset = new Vector3(0f, 15f, 0f); // HACK: Hard-coded, get this value from shark's zoom level
        }
    }
    
    [ClientRpc]
    public void SetupLocalPlayerControllerClientRpc(ulong clientID, NetworkObjectReference playerInstanceRef,  NetworkObjectReference playerControllerRef)
    {
        GameObject playerInstance = playerInstanceRef;
        PlayerController playerController = ((GameObject)playerControllerRef).GetComponent<PlayerController>();
        playerController.controlled = playerInstance;
    }

    [ClientRpc]
    public void AssignPlayerStatsUIClientRpc(NetworkObjectReference playerInstanceRef, LobbyPlayerData lobbyPlayerData)
    {
        GameObject playerInstance = playerInstanceRef;
        TextMeshPro textMesh;

        GameObject playerStatsUI = Instantiate(playerStatsUIPrefab, playerInstance.transform);
        
        textMesh = playerStatsUI.GetComponentInChildren<TextMeshPro>();

        textMesh.text = lobbyPlayerData.PlayerName + " " + lobbyPlayerData.ClientIPAddress;
    }
    
    public void StartLevelSelect()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            RaiseChangeLobbyVisibilityClientRpc(false); // Hide lobby UI
            
            //RaiseChangeLevelsVisibilityClientRpc(true); // Host selects level.
            OnChangeLevelsVisibility?.Invoke(true); // Only host selects level
        }
    }
    
    public void StartGame()
    {
        //spawn players
        foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(player);
            
            // OLD CODE
            //There has to be a less fragile way of linking the prefab to the index?
            // if (NetworkManager.Singleton.ConnectedClients[player].PlayerObject
            //     .GetComponent<PlayerController>()
            //     .selectedCharacter == characterSelect.CharacterIndex[0])
            // {
            //     GameObject go = Instantiate(test.sharkPrefab);
            //     go.GetComponent<NetworkObject>().Spawn();
            //     go.GetComponent<NetworkObject>().ChangeOwnership(player);
            // }
            //
            // else if (NetworkManager.Singleton.ConnectedClients[player].PlayerObject
            //     .GetComponent<PlayerController>()
            //     .selectedCharacter == characterSelect.CharacterIndex[1])
            // {
            //     GameObject go = Instantiate(test.fishPrefab);
            //     go.GetComponent<NetworkObject>().Spawn();
            //     go.GetComponent<NetworkObject>().ChangeOwnership(player);
            // }
        }

        RaiseStartEventClientRpc(); // TODO: People subscribing to this event should spawn characters/players for the level
    }

    public void SpawnPlayer(ulong clientID)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        
        Debug.Log("ID " + clientID + "; " + "Char = " + playerController.selectedCharacter);
        
        GameObject spawnedCharacter = Instantiate(playerController.selectedCharacter);
        playerController.controlled = spawnedCharacter;
            
        spawnedCharacter.GetComponent<NetworkObject>().Spawn();
        spawnedCharacter.GetComponent<NetworkObject>().ChangeOwnership(clientID);
        NetworkObjectReference characterReference = spawnedCharacter;
        SetupCameraClientRpc(clientID, characterReference);
        
        if (IsServer)
        {
            NetworkObjectReference playerControllerReference = playerController.gameObject;
            //SetupLocalPlayerControllerServerRpc(player, characterReference, playerControllerReference);
            SetupLocalPlayerControllerClientRpc(clientID, characterReference, playerControllerReference);
        }
        
        // We have a LobbyPlayerData for the current player created by the client.
        LobbyPlayerData lobbyPlayerData = networkList.GetPlayerDataByClientId(clientID);
        playerController.clientInfo = new NetworkVariable<LobbyPlayerData>(lobbyPlayerData); // Store the client info for now.
        playerController.playerColour = new NetworkVariable<Color>(RandomColour()); // Assign a random colour to the player for now.
        
        AssignPlayerStatsUIClientRpc(characterReference, lobbyPlayerData);
    }

    [ClientRpc]
    public void RaiseChangeLobbyVisibilityClientRpc(bool visibility)
    {
        OnChangeLobbyVisibility?.Invoke(visibility);
    }
    
    [ClientRpc]
    public void RaiseChangeLevelsVisibilityClientRpc(bool visibility)
    {
        OnChangeLevelsVisibility?.Invoke(visibility);
    }
    
    [ClientRpc]
    public void RaiseStartEventClientRpc()
    {
        StartedGameEvent?.Invoke();
    }
    
    public Color RandomColour()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
}