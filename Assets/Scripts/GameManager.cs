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
    public CameraFollow camera;
    
    public NetworkVariable<NetworkableString> selectedLevel { get; set; } = new NetworkVariable<NetworkableString>(NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> hasGameStarted { get; set; } = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone);
    public event Action StartedGameEvent;

    public event Action<bool> OnChangeLobbyVisibility;
    public event Action<bool> OnChangeLevelsVisibility;
    public event Action<bool> OnChangeInGameUIVisibility;

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
    public void SetupLocalPlayerControllerClientRpc(ulong clientID, NetworkObjectReference playerInstanceRef,
        NetworkObjectReference playerControllerRef)
    {
        GameObject playerInstance = playerInstanceRef;
        PlayerController playerController = ((GameObject) playerControllerRef).GetComponent<PlayerController>();
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
        StartGame();

        // Remove Cameras from loaded scenes
        UILevelsViewModel.Instance.DestroyCameras();
    }

    [ClientRpc]
    public void DestroyObjClientRpc(NetworkObjectReference objRef)
    {
        GameObject obj = objRef;
        
        Destroy(obj);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void JoinExistingSessionServerRpc(ulong clientId)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        
        PlayerController playerController = playerObject.GetComponent<PlayerController>();

        Debug.Log("ID " + clientId + "; " + "NEW Char = " + playerController.selectedCharacter);

        if (playerController.controlled != null)
        {
            // Destroy character selected previously. 
            NetworkObjectReference oldSpawnedControllableRef = playerController.controlled;

            DestroyObjClientRpc(oldSpawnedControllableRef);
        }
        
        GameObject spawnedCharacter = Instantiate(playerController.selectedCharacter);
        playerController.controlled = spawnedCharacter;

        spawnedCharacter.GetComponent<NetworkObject>().Spawn();
        spawnedCharacter.GetComponent<NetworkObject>().ChangeOwnership(clientId);
        NetworkObjectReference characterReference = spawnedCharacter;
        SetupCameraClientRpc(clientId, characterReference);

        NetworkObjectReference playerControllerReference = playerController.gameObject;
        SetupLocalPlayerControllerClientRpc(clientId, characterReference, playerControllerReference);

        // We have a LobbyPlayerData for the current player created by the client.
        LobbyPlayerData lobbyPlayerData = NetworkPlayerList.Instance.GetPlayerDataByClientId(clientId);
        playerController.clientInfo = new NetworkVariable<LobbyPlayerData>(lobbyPlayerData); // Store the client info for now.
        playerController.playerColour = new NetworkVariable<Color>(RandomColour()); // Assign a random colour to the player for now.

        // Spawn the player stats UI (name/IP) as a child of the current spawned character.
        AssignPlayerStatsUI(spawnedCharacter, lobbyPlayerData, clientId);

        // Show the In Game UI just for this client
        RaiseChangeInGameUIVisibilityClientRpc(true, clientId);
    }

    public void SinglePlayerJoin(NetworkObject playerObject, ulong player)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            PlayerController playerController = playerObject.GetComponent<PlayerController>();

            Debug.Log("ID " + player + "; " + "Char = " + playerController.selectedCharacter);

            GameObject spawnedCharacter = Instantiate(playerController.selectedCharacter);
            playerController.controlled = spawnedCharacter;

            spawnedCharacter.GetComponent<NetworkObject>().Spawn();
            spawnedCharacter.GetComponent<NetworkObject>().ChangeOwnership(player);
            NetworkObjectReference characterReference = spawnedCharacter;
            SetupCameraClientRpc(player, characterReference);

            NetworkObjectReference playerControllerReference = playerController.gameObject;
            SetupLocalPlayerControllerClientRpc(player, characterReference, playerControllerReference);

            // We have a LobbyPlayerData for the current player created by the client.
            LobbyPlayerData lobbyPlayerData = NetworkPlayerList.Instance.GetPlayerDataByClientId(player);
            playerController.clientInfo = new NetworkVariable<LobbyPlayerData>(lobbyPlayerData); // Store the client info for now.
            playerController.playerColour = new NetworkVariable<Color>(RandomColour()); // Assign a random colour to the player for now.

            // Spawn the player stats UI (name/IP) as a child of the current spawned character.
            AssignPlayerStatsUI(spawnedCharacter, lobbyPlayerData, player);

            // Show the In Game UI just for this client
            RaiseChangeInGameUIVisibilityClientRpc(true, player);

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
    }

    public void RaiseChangeLobbyVisibility(bool visibility, bool updateAllClients = false)
    {
        if (updateAllClients)
        {
            RaiseChangeLobbyVisibilityClientRpc(visibility);
        }
        else
        {
            OnChangeLobbyVisibility?.Invoke(visibility);
        }
    }
    
    public void RaiseChangeInGameUIVisibility(bool visibility, bool updateAllClients = false)
    {
        if (updateAllClients)
        {
            RaiseChangeInGameUIVisibilityClientRpc(visibility);
        }
        else
        {
            OnChangeInGameUIVisibility?.Invoke(visibility);
        }
    }

    [ClientRpc]
    public void RaiseChangeInGameUIVisibilityClientRpc(bool visibility)
    {
        OnChangeInGameUIVisibility?.Invoke(visibility);
    }
    
    [ClientRpc]
    public void RaiseChangeInGameUIVisibilityClientRpc(bool visibility, ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            OnChangeInGameUIVisibility?.Invoke(visibility);
        }
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