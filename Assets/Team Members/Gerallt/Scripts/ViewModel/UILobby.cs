using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine.UI;

namespace Gerallt
{
    public class UILobby : MonoBehaviour
    {
        public GameObject view;
        public GameObject JoinedClients;
        public GameObject UIClientPrefab;
        public TMP_InputField UIPlayerNameInput;
        public TMP_InputField UIServerIPInput;
        public Button buttonStartGame;
        public Button buttonHostGame;
        public Button buttonJoinGame;

        public LobbyPlayerData? localPlayerData;
        
        public void PlayerName_ValueChanged()
        {
            string playerName = UIPlayerNameInput.text;
            ulong clientId = NetworkManager.Singleton.LocalClientId;

            LobbyPlayerData playerData = NetworkPlayerList.Instance.GetPlayerDataByClientId(clientId);
            playerData.PlayerName = playerName;

            localPlayerData = playerData;

            if (clientId != ulong.MaxValue) // If has a valid client ID 
            {
                // Update local and server model.
                for (int i = 0; i < NetworkPlayerList.Instance.NetworkedObjects.Count; i++)
                {
                    LobbyPlayerData playerData2 = NetworkPlayerList.Instance.NetworkedObjects[i];
                
                    if (playerData2.ClientId == clientId)
                    {
                        playerData2.PlayerName = playerName;
                        playerData2.ClientIPAddress = NetworkPlayerList.GetClientIPAddress();
                
                        NetworkPlayerList.Instance.UpdatePlayerData(clientId, i, playerData2);
                        break;
                    }
                }
                
                //NetworkPlayerList.Instance.UpdatePlayerData(clientId, playerData);
            }

            // Update the view given model changes.
            UpdateClientsList();
        }

        public void ServerIP_ValueChanged()
        {
	        string serverIP = UIServerIPInput.text;

	        ServerManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = serverIP;
        }

        public void Start()
        {
            buttonHostGame.gameObject.SetActive(true);
            buttonStartGame.gameObject.SetActive(false);
            buttonJoinGame.gameObject.SetActive(true);  
            
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.OnChangeLobbyVisibility += GameManager_OnChangeLobbyVisibility;
                gameManager.StartedGameEvent += GameManager_OnStartedGameEvent;
            }
            
            NetworkPlayerList.Instance.NetworkedObjects.OnListChanged += NetworkedObjectsOnOnListChanged;
            
            //Setting the joining IP address to Cam's by default; reflecting the address in the UI input field
            string camsIP = "121.200.8.114";
            ServerManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = camsIP;
            UIServerIPInput.text = camsIP;
        }

        private void GameManager_OnChangeLobbyVisibility(bool visibility)
        {
            view.SetActive(visibility);

            if (visibility)
            {
                if (GameManager.Instance.hasGameStarted.Value)
                {
                    if (NetworkManager.Singleton.IsHost)
                    {
                        buttonHostGame.gameObject.SetActive(false);
                        buttonStartGame.gameObject.SetActive(false);
                        buttonJoinGame.gameObject.SetActive(true);
                    }
                    else if (NetworkManager.Singleton.IsClient)
                    {
                        buttonHostGame.gameObject.SetActive(false);
                        buttonStartGame.gameObject.SetActive(false);
                        buttonJoinGame.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (!NetworkManager.Singleton.IsHost && NetworkManager.Singleton.IsConnectedClient)
                    {
                        buttonHostGame.gameObject.SetActive(false);
                        buttonStartGame.gameObject.SetActive(false);
                        buttonJoinGame.gameObject.SetActive(true);
                    }
                    else
                    {
                        buttonHostGame.gameObject.SetActive(true);
                        buttonStartGame.gameObject.SetActive(false);
                        buttonJoinGame.gameObject.SetActive(true);   
                    }
                }   
            }
        }

        private void GameManager_OnStartedGameEvent()
        {
            view.SetActive(false);
        }

        public void OnDestroy()
        {
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.StartedGameEvent -= GameManager_OnStartedGameEvent;
            }
            
            NetworkPlayerList.Instance.NetworkedObjects.OnListChanged -= NetworkedObjectsOnOnListChanged;
        }

        public void OnJoinButtonClicked()
        {
            buttonStartGame.gameObject.SetActive(false);
            buttonHostGame.gameObject.SetActive(false);
            buttonJoinGame.gameObject.SetActive(false);

            ServerManager serverManager = ServerManager.Singleton as ServerManager;
            serverManager.JoinServer(localPlayerData, autoCreateHost: true);
        }

        public void StartHost_ButtonClicked()
        {
            buttonStartGame.gameObject.SetActive(true);
            buttonHostGame.gameObject.SetActive(false);
            buttonJoinGame.gameObject.SetActive(false);
            
            ServerManager serverManager = ServerManager.Singleton as ServerManager;
            serverManager.Host(localPlayerData);
        }

        public void StartGame_ButtonClicked()
        {
            buttonStartGame.gameObject.SetActive(false);
            buttonHostGame.gameObject.SetActive(false);
            buttonJoinGame.gameObject.SetActive(false);
            
            ServerManager serverManager = ServerManager.Singleton as ServerManager;
            serverManager.StartGame();
        }
        
        private void NetworkedObjectsOnOnListChanged(Unity.Netcode.NetworkListEvent<LobbyPlayerData> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<LobbyPlayerData>.EventType.Add:
                case NetworkListEvent<LobbyPlayerData>.EventType.Remove:
                case NetworkListEvent<LobbyPlayerData>.EventType.RemoveAt:
                case NetworkListEvent<LobbyPlayerData>.EventType.Insert:
                case NetworkListEvent<LobbyPlayerData>.EventType.Clear:
                case NetworkListEvent<LobbyPlayerData>.EventType.Value:
                    UpdateClientsList();
                    break;
            }
        }


        public void UpdateClientsList()
        {
            // Destroy all client UI instances in joined clients list
            for (int idx = 0; idx < JoinedClients.transform.childCount; idx++ )
            {
                Transform child = JoinedClients.transform.GetChild(idx);
                
                Destroy(child.gameObject);
            }

            if (NetworkPlayerList.Instance.NetworkedObjects != null)
            {
                // Refresh client UI with newly connected clients:
                List<LobbyPlayerData> duplicates = new List<LobbyPlayerData>();
                
                foreach(LobbyPlayerData lobbyPlayerData in NetworkPlayerList.Instance.NetworkedObjects)
                {
                    if (!duplicates.Any(dup => dup.ClientId == lobbyPlayerData.ClientId))
                    {
                        GameObject clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
                        UIClient uiClient = clientInstance.GetComponent<UIClient>();
                        uiClient.parentView = this;
                        uiClient.UpdateUI(lobbyPlayerData);
                    
                        duplicates.Add(lobbyPlayerData);   
                    }
                }
            }
        }
    }
}