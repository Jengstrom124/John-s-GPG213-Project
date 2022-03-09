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
    public void JoinServer(ulong client)
    {
	    

	    client = NetworkManager.Singleton.ServerClientId;
        
        //Pass in ClientId
        JoinServerEvent?.Invoke(client);
        
        //need to add client to list
		ClientList.Add(client);
    }

    //Starting Game, waiting on event from lobby
    public void StartGame()
    {
	    Debug.Log(NetworkManager.Singleton.ConnectedClients);
	    Debug.Log(ClientList.Count);

	    //spawn players
        foreach (var player in ClientList)
        {
	        
	        Instantiate(NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.ServerClientId].PlayerObject.GetComponent<PlayerController>()
	            .selectedCharacter);

	        //I feel dirty using strings
	        if (GetComponent<CharacterBase>().characterName == "Shark")
	        {
		        /*NetworkObject shark = Instantiate(sharkPrefab);
		        shark.GetComponent<NetworkObject>().ChangeOwnership(player);*/
	        }

	        if (GetComponent<CharacterBase>().characterName == "Fish")
	        {
		        /*NetworkObject fish = Instantiate(fishPrefab);
		        fish.GetComponent<NetworkObject>().ChangeOwnership(player);*/
	        }
        }
    }

    #region GUI Buttons
    void StartButtons()
    {
	    if (GUILayout.Button("Host"))
	    {
		    StartHost();
		    JoinServer(NetworkManager.Singleton.ServerClientId);
	    }
	    
        if(GUILayout.Button("Client"))
        {
	        StartClient();
	        JoinServer(NetworkManager.Singleton.ServerClientId);
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