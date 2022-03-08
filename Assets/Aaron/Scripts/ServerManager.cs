using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ServerManager : NetworkManager
{
    public event Action JoinServerEvent;

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
    public void JoinServer()
    {
        if (!IsHost && !IsServer)
        {
            StartClient();
        }

        //Event for something? Maybe providing client address? Not sure to be honest
        JoinServerEvent?.Invoke();
    }

    //Starting Game, waiting on event from lobby
    public void StartGame()
    {
        //spawn players
        foreach (var player in ConnectedClients)
        {
            //if player selected shark, spawn player as shark with ownership
            
            //elseif player selected fish, spawn player as fish with ownership
        }

        //Player Prefab; shark/fish (character) to Players
        //assign ownership
    }

    #region GUI Buttons
    void StartButtons()
    {
        if(GUILayout.Button("Host")) StartHost();
        if(GUILayout.Button("Client")) StartClient();
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