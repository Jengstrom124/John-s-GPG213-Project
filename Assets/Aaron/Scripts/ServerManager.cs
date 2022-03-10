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
    public event Action<int> JoinServerEvent;

    //need list of clients
    //public NetworkList<int> ClientList = new NetworkList<int>();

    /*public NetworkObject sharkPrefab;
    public NetworkObject fishPrefab;*/

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10,10,300,300));

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
	    Debug.Log(NetworkManager.Singleton.ConnectedClients.Count);

	    //spawn players
        foreach (var player in ConnectedClients)
        {
	        
	        Instantiate(NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.ServerClientId].PlayerObject.
		        GetComponent<PlayerController>().selectedCharacter);

	        //I feel dirty using strings
	        /*if (GetComponent<CharacterBase>().characterName == "Shark")
	        {
		        Instantiate(NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.ServerClientId].PlayerObject.GetComponent<PlayerController>()
			        .selectedCharacter);
		        /*NetworkObject shark = Instantiate(sharkPrefab);
		        shark.GetComponent<NetworkObject>().ChangeOwnership(player);#1#
	        }

	        if (GetComponent<CharacterBase>().characterName == "Fish")
	        {
		        Instantiate(NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.ServerClientId].PlayerObject.GetComponent<PlayerController>()
			        .selectedCharacter);
		        /*NetworkObject fish = Instantiate(fishPrefab);
		        fish.GetComponent<NetworkObject>().ChangeOwnership(player);#1#
	        }*/
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
	    
        if(GUILayout.Button("Client"))
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