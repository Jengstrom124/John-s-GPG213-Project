using System;
using System.Collections;
using System.Collections.Generic;
using Gerallt;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class ServerManager : NetworkManager
{
    private CharacterSelect characterSelect;
    public Test test;
    public Vector3 spawn;
    public LobbyPlayerData? currentPlayer;
    public string serverPassword = "room password";

    //passing a ulong for ClientId?
    public event Action<ulong> JoinServerEvent;

    private bool isServerUp = false;
    [SerializeField] private float connectionTimeout = 0.5f;

    // private void OnGUI()
    // {
    //     GUILayout.BeginArea(new Rect(10, 10, 300, 300));
    //
    //     if (!IsClient && !IsServer)
    //     {
    //         StartButtons();
    //     }
    //
    //     if (IsServer || IsHost)
    //     {
    //         StopButton();
    //     }
    //
    //     GUILayout.EndArea();
    // }

    // Start is called before the first frame update
    void Start()
    {
        //sub to StartGameEvent from lobby; call StartGame()

        NetworkManager.Singleton.RunInBackground = true;
        
        //How else to grab this besides FindObject?
        characterSelect = FindObjectOfType<CharacterSelect>();
        test = FindObjectOfType<Test>();

        //OnClientConnectedCallback += OnConnectedCallback;
        
        OnClientConnectedCallback += ServerManager_OnClientConnectedCallback;
    }

    public void OnConnectedToServer()
    {
        //OnClientConnectedCallback += ServerManager_OnClientConnectedCallback;
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

        
        
        if (autoCreateHost)
        {
            Client();

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
            //     Client();
            // }
            
            ulong client = this.LocalClientId;

            // SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
            // SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
        }
        

        
        //Pass in ClientId
        JoinServerEvent?.Invoke(LocalClientId);
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
        sceneEvent.LoadSceneMode = LoadSceneMode.Additive;
        
        switch (sceneEvent.SceneEventType)
        {
            //case SceneEventType.SynchronizeComplete: // Clients have loaded all scenes that were loaded by the server.
            //case SceneEventType.LoadEventCompleted:
            case SceneEventType.LoadComplete:
                string sceneName = sceneEvent.SceneName;
                if (sceneName != UILevelsViewModel.managerScene)
                {
                    SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;

                    LoadSceneEventServerRpc(sceneName);
                    //UILevelsViewModel.Instance.LoadEventCompleted(sceneName, true);
                    
                    //UILevelsViewModel.Instance.StartPlayerServerRpc();
                }

                break;
        }
    }

    [ServerRpc]
    private void LoadSceneEventServerRpc(string sceneName)
    {
        UILevelsViewModel.Instance.LoadEventCompleted(sceneName, true);
    }

    private IEnumerator ClientConnectedCoroutine(LobbyPlayerData lobbyPlayerData)
    {
        yield return new WaitForSeconds(1.0f);
        NetworkPlayerList.Instance.UpdatePlayerDataServerRpc(lobbyPlayerData);
    }
    
    private void ServerManager_OnClientConnectedCallback(ulong clientId)
    {
        if (currentPlayer.HasValue)
        {
            LobbyPlayerData lobbyPlayerData = currentPlayer.Value;
            
            lobbyPlayerData.ClientId = clientId;
            
            StartCoroutine(ClientConnectedCoroutine(lobbyPlayerData));
        }
        
        if (GameManager.Instance.hasGameStarted.Value)
        {
            if (currentPlayer.HasValue)
            {
                //NetworkPlayerList.Instance.UpdatePlayerDataServerRpc(currentPlayer.Value);
            }

            GameManager.Instance.StartPlayerServerRpc(clientId);
        }
        
        currentPlayer = null;
    }

    // IEnumerator ConnectionTimeout()
    // {
    //     yield return new WaitForSeconds(connectionTimeout);
    //     if (!isServerUp)
    //     {
    //         Shutdown();
    //         yield return new WaitForSeconds(1.0f);
    //         GNetworkState state = FindObjectOfType<GNetworkState>();
    //         state.Init(FindObjectOfType<ServerManager>()); // Respawn the network list as it got destroyed on shutdown.
    //         
    //         StartHost();
    //         yield return new WaitForSeconds(1.0f);
    //         //state.Spawn();
    //     }
    // }

    public void Host(LobbyPlayerData? localPlayerData)
    {
        currentPlayer = localPlayerData;
        
        ConnectionApprovalCallback += OnConnectionApprovalCallback;
        NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(serverPassword);
        
        StartHost();
        
        SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
    }

    public void Client()
    {
        bool alreadyConnected = false;
        
        if (LocalClientId != ulong.MaxValue)
        {
            if (NetworkPlayerList.Instance.HasJoined(LocalClientId))
            {
                alreadyConnected = true;
            }
        }

        if (alreadyConnected)
        {
            GameManager.Instance.RaiseChangeLobbyVisibility(false, false);
            GameManager.Instance.RaiseChangeInGameUIVisibility(true);

            // Tell the server that this client wants to rejoin to their existing session.
            GameManager.Instance.JoinExistingSessionServerRpc(LocalClientId, ulong.MaxValue);
        }
        else
        {
            NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(serverPassword);

            StartClient();
            
            GameManager.Instance.RaiseChangeLobbyVisibility(false, false);
            GameManager.Instance.RaiseChangeInGameUIVisibility(true);
        }
    }
    
    private void OnConnectionApprovalCallback(byte[] connectionData, ulong clientId, ConnectionApprovedDelegate connectionApprovedDelegate)
    {
        bool approve = true;
        bool createPlayerObject = true;

        string password = System.Text.Encoding.UTF8.GetString(connectionData);

        if (password != serverPassword)
        {
            approve = false;
        }
        
        // Position to spawn the player object at, set to null to use the default position
        Vector3? positionToSpawnAt = null;

        // Rotation to spawn the player object at, set to null to use the default rotation
        Quaternion? rotationToSpawnWith = null;

        connectionApprovedDelegate(createPlayerObject, null, approve, positionToSpawnAt, rotationToSpawnWith);
    }
    
    //Starting Game, waiting on event from lobby
    public void StartGame()
    {
        GameManager.Instance.StartLevelSelect();
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