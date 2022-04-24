using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace Gerallt
{
    public class UILobby : MonoBehaviour
    {
        public GameObject view;
        public GameObject JoinedClients;
        public GameObject UIClientPrefab;
        public TMP_InputField UIPlayerNameInput;

        public void PlayerName_ValueChanged()
        {
            string playerName = UIPlayerNameInput.text;
            ulong clientId = NetworkManager.Singleton.LocalClientId;

            LobbyPlayerData playerData = NetworkPlayerList.Instance.GetPlayerDataByClientId(clientId);
            playerData.PlayerName = playerName;

            // Update local and server model.
            for (int i = 0; i < NetworkPlayerList.Instance.NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData2 = NetworkPlayerList.Instance.NetworkedObjects[i];

                if (playerData2.ClientId == clientId)
                {
                    playerData2.PlayerName = playerName;
                    playerData2.ClientIPAddress = NetworkPlayerList.GetClientIPAddress();

                    NetworkPlayerList.Instance.UpdatePlayerData(i, playerData2);
                    break;
                }
            }

            // Update the view given model changes.
            UpdateClientsList();
        }

        public void Start()
        {
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.OnChangeLobbyVisibility += GameManager_OnChangeLobbyVisibility;
                gameManager.StartedGameEvent += GameManager_OnStartedGameEvent;
            }
            
            NetworkPlayerList.Instance.NetworkedObjects.OnListChanged += NetworkedObjectsOnOnListChanged;
        }

        private void GameManager_OnChangeLobbyVisibility(bool visibility)
        {
            view.SetActive(visibility);
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
            //LobbyPlayerData lobbyPlayerData = GNetworkedListBehaviour.GetPlayerData();
            
            LobbyPlayerData? lobbyPlayerData = null;
            
            ServerManager serverManager = ServerManager.Singleton as ServerManager;
            serverManager.JoinServer(lobbyPlayerData, autoCreateHost: true);
        }

        public void StartHost_ButtonClicked()
        {
            ServerManager serverManager = ServerManager.Singleton as ServerManager;
            serverManager.Host();
        }

        public void StartGame_ButtonClicked()
        {
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