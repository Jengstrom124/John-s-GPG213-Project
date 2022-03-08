using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ServerManager : NetworkManager
{
	//passing a ulong for ClientId?
    public event Action<ulong> JoinServerEvent;

    //need list of clients
    public List<ulong> ClientList = new List<ulong>();

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10,10,300,300));

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
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
    }

    //Called from lobby
    public void JoinServer(ulong client)
    {
        if (!IsHost && !IsServer)
        {
            StartClient();
        }

        //Pass in ClientId
        JoinServerEvent?.Invoke(client);
        
        //need to add client to list
		ClientList.Add(client);
    }

    //Starting Game, waiting on event from lobby
    public void StartGame()
    {
        //spawn players
        foreach (var player in ClientList)
        {
	        Instantiate(NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.ServerClientId].PlayerObject.GetComponent<PlayerController>()
	            .selectedCharacter);
        }
    }

    #region GUI Buttons
    void StartButtons()
    {
        if(GUILayout.Button("Host")) StartHost();
        if (GUILayout.Button("Client"))
        {
	        StartClient();
	        JoinServer(NetworkManager.Singleton.LocalClientId);
        }
        if(GUILayout.Button("Server")) StartServer();
    }

    void StopButton()
    {
        if (GUILayout.Button("Stop Server")) Shutdown();
    }

    #endregion
    
    public void LobbyStartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}