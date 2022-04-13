using System;
using System.Collections;
using System.Collections.Generic;
using Gerallt;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class ServerManager : NetworkManager
{
    private CharacterSelect characterSelect;
    public Test test;
    public Vector3 spawn;
    public LobbyPlayerData? currentPlayer;
    
    [SerializeField] private GNetworkedListBehaviour networkedListBehaviour;
    
    //passing a ulong for ClientId?
    public event Action<ulong> JoinServerEvent;

    private bool isServerUp = false;
    [SerializeField] private float connectionTimeout = 0.5f;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (!IsClient && !IsServer)
        {
            StartButtons();
        }

        if (IsServer || IsHost)
        {
            StopButton();
        }

        GUILayout.EndArea();
    }

    // Start is called before the first frame update
    void Start()
    {
        //sub to StartGameEvent from lobby; call StartGame()

        NetworkManager.Singleton.RunInBackground = true;
        
        //How else to grab this besides FindObject?
        characterSelect = FindObjectOfType<CharacterSelect>();
        test = FindObjectOfType<Test>();
        networkedListBehaviour = FindObjectOfType<GNetworkedListBehaviour>();
        
        //OnClientConnectedCallback += OnConnectedCallback;
    }

    public NetworkObject Resolve(NetworkObjectReference networkObjectRef)
    {
        NetworkManager networkManager = this;
        
        ulong key = networkObjectRef.NetworkObjectId;
        
        return Resolve(key);
    }
    
    public NetworkObject Resolve(ulong keyClientId)
    {
        NetworkManager networkManager = this;


        if (keyClientId != LocalClientId)
            return null;
        
        //networkManager.SpawnManager.SpawnedObjects.TryGetValue(keyClientId, out NetworkObject networkObject);

        NetworkObject networkObject = networkManager.SpawnManager.GetPlayerNetworkObject(keyClientId);
        
        return networkObject;
    }

    //Called from lobby
    public void JoinServer(LobbyPlayerData? currentPlayerData = null, bool autoCreateHost = false)
    {
        currentPlayer = currentPlayerData;
        
        //client = NetworkManager.Singleton.GetInstanceID();

        if (autoCreateHost)
        {
            StartClient();
            
            // // Try to connect to server, if it fails connecting then start a new host.
            // // If the server hasn't been created, then StartHost() otherwise StartClient()
            //
            // if (!IsClient && !IsServer)
            // {
            //     // Quick test to see if the server is up.
            //     OnClientConnectedCallback+= delegate(ulong clientID)
            //     {
            //         isServerUp = true;
            //     };
            //
            //     StartCoroutine(ConnectionTimeout());
            //     
            //     StartClient();
            // }
        }
        
        ulong client = this.LocalClientId;
        
        //Pass in ClientId
        JoinServerEvent?.Invoke(client);
    }

    IEnumerator ConnectionTimeout()
    {
        yield return new WaitForSeconds(connectionTimeout);
        if (!isServerUp)
        {
            Shutdown();
            yield return new WaitForSeconds(1.0f);
            GNetworkState state = FindObjectOfType<GNetworkState>();
            state.Init(FindObjectOfType<ServerManager>()); // Respawn the network list as it got destroyed on shutdown.
            
            StartHost();
            yield return new WaitForSeconds(1.0f);
            //state.Spawn();
        }
    }

    public Color RandomColour()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
    }
    
    //Starting Game, waiting on event from lobby
    public void StartGame()
    {

	    //spawn players
        foreach (var player in ConnectedClientsIds)
        {
            Debug.Log("ID " + player + "; " + "Char = " + ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>()
                .selectedCharacter);

            PlayerController playerController = ConnectedClients[player].PlayerObject.GetComponent<PlayerController>();
            
            Instantiate(ConnectedClients[player].PlayerObject.GetComponent<PlayerController>().selectedCharacter);

            LobbyPlayerData lobbyPlayerData = networkedListBehaviour.GetPlayerDataByClientId(player);
            
            // We have a LobbyPlayerData for the current player created by the client.
            playerController.clientInfo = lobbyPlayerData; // Store the client info for now.
            playerController.playerColour = new NetworkVariable<Color>(RandomColour()); // Assign a random colour to the player for now.
            playerController.playerName = lobbyPlayerData.PlayerName; // Player name doesn't have to be networked anymore.

            //There has to be a less fragile way of linking the prefab to the index?
            if (ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>()
                .selectedCharacter == characterSelect.CharacterIndex[0])
            {
                GameObject go = Instantiate(test.sharkPrefab);
                go.GetComponent<NetworkObject>().Spawn();
                go.GetComponent<NetworkObject>().ChangeOwnership(player);
            }

            else if (ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>()
                .selectedCharacter == characterSelect.CharacterIndex[1])
            {
                GameObject go = Instantiate(test.fishPrefab);
                go.GetComponent<NetworkObject>().Spawn();
                go.GetComponent<NetworkObject>().ChangeOwnership(player);
            }
        }
    }

    #region GUI Buttons

    void StartButtons()
    {
        if (GUILayout.Button("Host"))
        {
            StartHost();
            JoinServer();
        }

        if (GUILayout.Button("Client"))
        {
            StartClient();
            JoinServer();
        }

        if (GUILayout.Button("Server"))
        {
            StartServer();
        }
    }

    void StopButton()
    {
        if (GUILayout.Button("Stop Server"))
        {
            Shutdown();
        }

        if (GUILayout.Button("Start Game"))
        {
            StartGame();
        }
    }

    #endregion
}