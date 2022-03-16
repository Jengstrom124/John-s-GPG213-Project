using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Tasks.Actions;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ServerManager : NetworkManager
{
    private CharacterSelect characterSelect;
    public Test test;
    public Vector3 spawn;

    //passing a ulong for ClientId?
    public event Action<int> JoinServerEvent;

    public NetworkList<NetworkObjectReference> NetworkedObjects = new NetworkList<NetworkObjectReference>();

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

        networkManager.SpawnManager.SpawnedObjects.TryGetValue(keyClientId, out NetworkObject networkObject);

        return networkObject;
    }
    
    
    
    private void OnConnectedCallback(ulong clientId)
    {
        NetworkObject spawnedObj = Resolve(clientId);

        if (spawnedObj != null)
        {
            NetworkedObjects.Add(new NetworkObjectReference(spawnedObj));
        }
    }

    //Called from lobby
    public void JoinServer(int client)
    {
        client = NetworkManager.Singleton.GetInstanceID();

        //Pass in ClientId
        JoinServerEvent?.Invoke(client);

        //need to add client to list
        //ClientList.Add(client);
    }

    //Starting Game, waiting on event from lobby
    public void StartGame()
    {
        Debug.Log(ConnectedClients.Count);

        //spawn players
        foreach (var player in ConnectedClientsIds)
        {
            Debug.Log("ID " + player + "; " + "Char = " + ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>()
                .selectedCharacter);

            Instantiate(ConnectedClients[player].PlayerObject.GetComponent<PlayerController>().selectedCharacter);

            //There has to be a less fragile way of linking the prefab to the index?
            if (ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>()
                .selectedCharacter == characterSelect.CharacterIndex[0])
            {
                Instantiate(test.sharkPrefab);
                //shark.GetComponent<NetworkObject>().Spawn();
                //shark.GetComponent<NetworkObject>().ChangeOwnership(player);
            }

            else if (ConnectedClients[player].PlayerObject
                .GetComponent<PlayerController>()
                .selectedCharacter == characterSelect.CharacterIndex[1])
            {
                Instantiate(test.fishPrefab);
                //fish.GetComponent<NetworkObject>().Spawn();
                //fish.GetComponent<NetworkObject>().ChangeOwnership(player);
            }
        }
    }

    #region GUI Buttons

    void StartButtons()
    {
        if (GUILayout.Button("Host"))
        {
            StartHost();
            JoinServer(NetworkManager.Singleton.GetInstanceID());
        }

        if (GUILayout.Button("Client"))
        {
            StartClient();
            JoinServer(NetworkManager.Singleton.GetInstanceID());
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

    public void LobbyStartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}