using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public ServerManager ServerManager;
        
        private NetworkPlayerList GNetworkedListBehaviour;
        
        public event Action<LobbyPlayerData> OnPlayerDataChanged;
        
        public void PlayerName_ValueChanged()
        {
            string playerName = UIPlayerNameInput.text;
            ulong clientId = ServerManager.LocalClientId;

            for (int i = 0; i < GNetworkedListBehaviour.NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData = GNetworkedListBehaviour.NetworkedObjects[i];

                if (playerData.ClientId == clientId)
                {
                    playerData.PlayerName = playerName;
                    playerData.ClientIPAddress = NetworkPlayerList.GetClientIPAddress();
                    //GNetworkedListBehaviour.UpdatePlayerName(i, playerName);
                    GNetworkedListBehaviour.UpdatePlayerData(i, playerData);
                    
                    OnPlayerDataChanged?.Invoke(playerData);
                    break;
                }
            }
        }

        public void Start()
        {
            GameManager gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.OnChangeLobbyVisibility += GameManager_OnChangeLobbyVisibility;
                gameManager.StartedGameEvent += GameManager_OnStartedGameEvent;
            }

            GNetworkedListBehaviour = NetworkPlayerList.Instance;
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged += NetworkedObjectsOnOnListChanged;
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
            
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged -= NetworkedObjectsOnOnListChanged;
        }

        public void OnJoinButtonClicked()
        {
            LobbyPlayerData lobbyPlayerData = GNetworkedListBehaviour.GetPlayerData();
            
            ServerManager.JoinServer(lobbyPlayerData, autoCreateHost: true);
        }
        
        private void NetworkedObjectsOnOnListChanged(Unity.Netcode.NetworkListEvent<LobbyPlayerData> changeEvent)
        {
            UpdateClientsList();
        }


        public void UpdateClientsList()
        {
            // Destroy all client UI instances in joined clients list
            for (int idx = 0; idx < JoinedClients.transform.childCount; idx++ )
            {
                Transform child = JoinedClients.transform.GetChild(idx);
                
                Destroy(child.gameObject);
            }

            if (GNetworkedListBehaviour.NetworkedObjects != null)
            {
                // Refresh client UI with newly connected clients:
                foreach(LobbyPlayerData lobbyPlayerData in GNetworkedListBehaviour.NetworkedObjects)
                {
                    GameObject clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
                    UIClient uiClient = clientInstance.GetComponent<UIClient>();
                    uiClient.parentView = this;
                    uiClient.UpdateUI(lobbyPlayerData);
                }
            }
        }
    }
}