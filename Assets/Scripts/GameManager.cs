using System;
using System.Collections;
using System.Collections.Generic;
using Tom;
using Unity.Netcode;
using UnityEngine;
using Gerallt;
using TMPro;
using Unity.Netcode.Components;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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

    public mayaSpawner mayaSpawner;
    
    public enum CharacterTypes
    {
        LukeShark,
        OllieShark,
        KevinSeal
    }

    public GameObject[] characterPrefabs;

    [ClientRpc]
    public void SetupCameraClientRpc(ulong clientID, NetworkObjectReference playerObjectRef)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            GameObject playerObject = playerObjectRef;
            camera.target = playerObject.transform;
            //camera.offset = new Vector3(0f, 15f, 0f); // HACK: Hard-coded, get this value from shark's zoom level
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
            playerStatsNetworkObj.SpawnWithOwnership(clientId);
            //playerStatsNetworkObj.ChangeOwnership(clientId);
            playerStatsNetworkObj.TrySetParent(playerInstance.transform, false);
            
            playerStatsUI.transform.parent = playerInstance.transform;
        }
        else
        {
            playerStatsUI.transform.parent = playerInstance.transform;
        }
        //playerStatsUI.transform.position = Vector3.zero;
        //playerStatsUI.transform.position = playerInstance.transform.position;

        NetworkObjectReference playerInstanceRef = playerInstance;
        NetworkObjectReference playerStatsUIRef = playerStatsUI;
        UpdateStatsClientRpc(playerInstanceRef, playerStatsUIRef, lobbyPlayerData);
    }

    [ClientRpc]
    public void UpdateStatsClientRpc(NetworkObjectReference playerInstanceRef, NetworkObjectReference playerStatsUIRef, LobbyPlayerData lobbyPlayerData)
    {
        GameObject playerStatsUIObj = playerStatsUIRef;
        PlayerStatsUI playerStatsUIModel = playerStatsUIObj.GetComponent<PlayerStatsUI>();
        playerStatsUIModel.UpdateStats(lobbyPlayerData);

        GameObject playerInstance = playerInstanceRef;
        
        playerStatsUIObj.transform.position = playerInstance.transform.position + playerStatsUIModel.offset;
        playerStatsUIObj.transform.rotation = playerInstance.transform.rotation;
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
            //foreach (var player in NetworkPlayerList.Instance.NetworkedObjects)
            {
                NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[player].PlayerObject;

                SinglePlayerJoin(playerObject, player, ulong.MaxValue);
            }

            RaiseStartEventClientRpc(); // TODO: People subscribing to this event should spawn characters/players for the level

            if (!hasGameStarted.Value && IsServer)
            {
                hasGameStarted.Value = true; // Didn't update late joining clients

                //SetGameStartedServerRpc();
            }
        }
    }

    public void StartGameUpdateLocalClient(ulong clientId)
    {
        // Replicate spawn players that are on the server locally
        if (ServerManager.Singleton.IsServer)
        {
            foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
            //foreach (var player in NetworkPlayerList.Instance.NetworkedObjects)
            {
                NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[player].PlayerObject;

                SinglePlayerJoin(playerObject, player, clientId, true, true);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartPlayerServerRpc(ulong clientId)
    {
        //StartGame();
        StartGameUpdateLocalClient(clientId);

        // Remove Cameras from loaded scenes
        UILevelsViewModel.Instance.DestroyCameras();
    }

    [ClientRpc]
    public void DestroyObjClientRpc(NetworkObjectReference objRef)
    {
        GameObject obj = objRef;

        // NetworkObject networkObject = obj.GetComponent<NetworkObject>();
        // if (networkObject != null)
        // {
        //     if (networkObject.IsSpawned)
        //     {
        //         networkObject.Despawn();
        //     }
        // }
        // else
        // {
            Destroy(obj);
        //}
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void JoinExistingSessionServerRpc(ulong clientId, ulong localClientId, bool lateJoin = false)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        bool spawnReady = true;
        
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        
        Vector3? oldPosition = null;
        Quaternion? oldRotation = null;
        
        if (lateJoin && localClientId != ulong.MaxValue)
        {
            if (playerController.controlled != null)
            {
                if (clientId == localClientId)
                {
                    oldPosition = playerController.controlled.transform.position;
                    oldRotation = playerController.controlled.transform.rotation;
                    
                    // Client might have selected a new character so have to recreate it
                    var playerStats = playerController.controlled.GetComponentInChildren<PlayerStatsUI>();
                    //DestroyObjClientRpc(playerStats.gameObject);
                    //DestroyObjClientRpc(playerController.controlled);

                    if (playerStats != null)
                    {
                        NetworkObject playerStatsNetworkObject = playerStats.GetComponent<NetworkObject>();
                        if (playerStatsNetworkObject != null && playerStatsNetworkObject.IsSpawned)
                        {
                            playerStatsNetworkObject.Despawn();
                        }
                    }
                    
                    NetworkObject networkObject = playerController.controlled.GetComponent<NetworkObject>();
                    if (networkObject != null && networkObject.IsSpawned)
                    {
                        networkObject.Despawn();
                    }
                    else if (networkObject == null)
                    {
                        DestroyObjClientRpc(playerController.controlled);
                    }

                    //playerController.controlled = null;
                }
                else
                {
                    // Always make sure PlayerController has a controlled object
                    // (Late joiners have this set as null)
                    NetworkObjectReference playerControllerReference = playerController.gameObject;
                    NetworkObjectReference characterReference = playerController.controlled;
                    SetupLocalPlayerControllerClientRpc(clientId, characterReference, playerControllerReference);
                    
                    // UPDATE VIEW since late joiners are out of sync
                    // We have a LobbyPlayerData for the current player created by the client.
                    LobbyPlayerData lobbyPlayerData = NetworkPlayerList.Instance.GetPlayerDataByClientId(clientId);

                    var playerStatsUI = playerController.controlled.GetComponentInChildren<PlayerStatsUI>().gameObject;
                    NetworkObjectReference playerStatsUIRef = playerStatsUI;
                    NetworkObjectReference playerInstanceRef = playerController.controlled;
                    UpdateStatsClientRpc(playerInstanceRef, playerStatsUIRef, lobbyPlayerData);
                    
                    return;
                }
            }
        }
        
        SetupPlayer(clientId, oldPosition, oldRotation);
    }

    public void SinglePlayerJoin(NetworkObject playerObject, ulong player, ulong clientId, bool justLocalClient = false, bool lateJoin = false)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (justLocalClient)
            {
                JoinExistingSessionServerRpc(player,clientId, lateJoin);
            }
            else
            {
                SetupPlayer(player, null, null);
            }
        }
    }

    public void SetupPlayer(ulong clientID, Vector3? position, Quaternion? rotation)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
        PlayerController playerController = playerObject.GetComponent<PlayerController>();

        Debug.Log("ID " + clientID + "; " + "Char = " + playerController.selectedCharacter);

        int characterIndex = NetworkPlayerList.Instance.GetPlayerDataByClientId(clientID).characterIndex;
        playerController.selectedCharacter = characterPrefabs[characterIndex];
        GameObject spawnedCharacter = Instantiate(playerController.selectedCharacter);
        
        Transform t = spawnedCharacter.transform;
        if (position.HasValue)
        {
            t.position = position.Value;
        }
        else
        {
            t.position = mayaSpawner.CalculateRandomPosition(mayaSpawner.WaterOrNot.Water);
        }
        if (rotation.HasValue)
        {
            t.rotation = rotation.Value;
        }
        else
        {
            t.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
        
        
        playerController.controlled = spawnedCharacter;

        NetworkObject spawnedCharacterNetworkObject = spawnedCharacter.GetComponent<NetworkObject>();

        if (spawnedCharacterNetworkObject == null)
        {
            spawnedCharacterNetworkObject = spawnedCharacter.AddComponent<NetworkObject>();
            spawnedCharacter.AddComponent<NetworkRigidbody>();
            spawnedCharacter.AddComponent<NetworkTransform>();
        }
        
        spawnedCharacterNetworkObject.SpawnWithOwnership(clientID);
        //spawnedCharacterNetworkObject.ChangeOwnership(clientID);

        NetworkObjectReference characterReference = spawnedCharacter;
        SetupCameraClientRpc(clientID, characterReference);

        NetworkObjectReference playerControllerReference = playerController.gameObject;
        SetupLocalPlayerControllerClientRpc(clientID, characterReference, playerControllerReference);

        // We have a LobbyPlayerData for the current player created by the client.
        LobbyPlayerData lobbyPlayerData = NetworkPlayerList.Instance.GetPlayerDataByClientId(clientID);
        playerController.clientInfo = new NetworkVariable<LobbyPlayerData>(lobbyPlayerData); // Store the client info for now.
        playerController.playerColour = new NetworkVariable<Color>(RandomColour()); // Assign a random colour to the player for now.

        // Spawn the player stats UI (name/IP) as a child of the current spawned character.
        AssignPlayerStatsUI(spawnedCharacter, lobbyPlayerData, clientID);

        // Show the In Game UI just for this client
        RaiseChangeInGameUIVisibilityClientRpc(true, clientID);
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