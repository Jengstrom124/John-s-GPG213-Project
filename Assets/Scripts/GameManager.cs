using System;
using System.Collections;
using System.Collections.Generic;
using Tom;
using Unity.Netcode;
using UnityEngine;
using Gerallt;

public class GameManager : ManagerBase<GameManager>
{
    public List<string> levels = new List<string>();
    public NetworkPlayerList networkList;

    public CameraFollow camera;

    public event Action StartedGameEvent;

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

    public void StartGame()
    {
        //spawn players
        foreach (var player in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log("ID " + player + "; " + "Char = " + NetworkManager.Singleton.ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>()
                .selectedCharacter);

            PlayerController playerController = NetworkManager.Singleton.ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>();

            GameObject spawnedCharacter = Instantiate(playerController.selectedCharacter);
            spawnedCharacter.GetComponent<NetworkObject>().Spawn();
            NetworkObjectReference characterReference = spawnedCharacter;
            SetupCameraClientRpc(player, characterReference);

            LobbyPlayerData lobbyPlayerData = networkList.GetPlayerDataByClientId(player);

            // We have a LobbyPlayerData for the current player created by the client.
            playerController.clientInfo = lobbyPlayerData; // Store the client info for now.
            playerController.playerColour =
                new NetworkVariable<Color>(RandomColour()); // Assign a random colour to the player for now.
            playerController.playerName =
                lobbyPlayerData.PlayerName; // Player name doesn't have to be networked anymore.

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

        //StartedGameEvent?.Invoke();
        if (NetworkManager.Singleton.IsServer)
        {
            RaiseStartEventClientRpc();
        }
    }


    [ClientRpc]
    public void RaiseStartEventClientRpc()
    {
        StartedGameEvent?.Invoke();
        
        // UILobby uiLobby = FindObjectOfType<UILobby>();
        //
        // if (uiLobby != null)
        // {
        //     uiLobby
        // }
    }
    
    public Color RandomColour()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
}