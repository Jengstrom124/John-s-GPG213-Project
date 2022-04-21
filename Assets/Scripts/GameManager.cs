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

    //public NetworkVariable<bool> hasGameStarted = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> hasGameStarted { get; set; } = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone);



    public CameraFollow camera;
    public event Action StartedGameEvent;

    public event Action<bool> OnChangeLobbyVisibility;
    public event Action<bool> OnChangeLevelsVisibility;

    public GameObject playerStatsUIPrefab;
    

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
    
    public void AssignPlayerStatsUI(GameObject playerInstance, LobbyPlayerData lobbyPlayerData, ulong clientId)
    {
        GameObject playerStatsUI = Instantiate(playerStatsUIPrefab);
        NetworkObject playerStatsNetworkObj = playerStatsUI.GetComponent<NetworkObject>();
        if (playerStatsNetworkObj != null)
        {
            playerStatsNetworkObj.Spawn();
            playerStatsNetworkObj.ChangeOwnership(clientId);
            playerStatsNetworkObj.TrySetParent(playerInstance.transform);
        }
        else
        {
            playerStatsUI.transform.parent = playerInstance.transform;
        }

        
        NetworkObjectReference playerStatsUIRef = playerStatsUI;
        UpdateStatsClientRpc(playerStatsUIRef, lobbyPlayerData);
    }

    [ClientRpc]
    public void UpdateStatsClientRpc(NetworkObjectReference playerStatsUIRef, LobbyPlayerData lobbyPlayerData)
    {
        GameObject playerStatsUIObj = playerStatsUIRef;
        PlayerStatsUI playerStatsUIModel = playerStatsUIObj.GetComponent<PlayerStatsUI>();
        
        playerStatsUIModel.UpdateStats(lobbyPlayerData);
    }
    
    public void StartLevelSelect()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            RaiseChangeLobbyVisibilityClientRpc(false); // Hide lobby UI
            
            //RaiseChangeLevelsVisibilityClientRpc(true); // Anyone can select level.
            OnChangeLevelsVisibility?.Invoke(true); // Only host selects level
        }
    }
    
    public void StartGame()
    {
        //spawn players
        if (ServerManager.Singleton.IsServer)
        {
            foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
            {
                NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[player].PlayerObject;

                SinglePlayerJoin(playerObject, player);
            }

            RaiseStartEventClientRpc(); // TODO: People subscribing to this event should spawn characters/players for the level

            if (!hasGameStarted.Value && IsServer)
            {
                hasGameStarted.Value = true; // Didn't update late joining clients

                //SetGameStartedServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartPlayerServerRpc(ulong clientId)
    {
        //NetworkObject playerObject = ServerManager.Singleton.ConnectedClients[clientId].PlayerObject;

        //SinglePlayerJoin(playerObject, clientId);

        StartGame();

        // Remove Cameras from loaded scenes
        UILevelsViewModel.Instance.DestroyCameras();
    }
    
    public void SinglePlayerJoin(NetworkObject playerObject, ulong player)
    {
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
            
        Debug.Log("ID " + player + "; " + "Char = " + playerController.selectedCharacter);
    
        GameObject spawnedCharacter = Instantiate(playerController.selectedCharacter);
        playerController.controlled = spawnedCharacter;
        
        spawnedCharacter.GetComponent<NetworkObject>().Spawn();
        spawnedCharacter.GetComponent<NetworkObject>().ChangeOwnership(player);
        NetworkObjectReference characterReference = spawnedCharacter;
        SetupCameraClientRpc(player, characterReference);
    
        if (IsServer)
        {
            NetworkObjectReference playerControllerReference = playerController.gameObject;
            //SetupLocalPlayerControllerServerRpc(player, characterReference, playerControllerReference);
            SetupLocalPlayerControllerClientRpc(player, characterReference, playerControllerReference);
        }
    
        // We have a LobbyPlayerData for the current player created by the client.
        LobbyPlayerData lobbyPlayerData = NetworkPlayerList.Instance.GetPlayerDataByClientId(player);
        playerController.clientInfo = new NetworkVariable<LobbyPlayerData>(lobbyPlayerData); // Store the client info for now.
        playerController.playerColour = new NetworkVariable<Color>(RandomColour()); // Assign a random colour to the player for now.

        if (IsServer)
        {
            AssignPlayerStatsUI(spawnedCharacter, lobbyPlayerData, player);
        }
        
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