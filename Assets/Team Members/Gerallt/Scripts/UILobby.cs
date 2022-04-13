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
        public GameObject JoinedClients;
        public GameObject UIClientPrefab;
        public TMP_InputField UIPlayerNameInput;
        
        public ServerManager ServerManager;
        public GNetworkedListBehaviour GNetworkedListBehaviour;
        
        public event Action OnGameStart;
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
                    playerData.ClientIPAddress = GNetworkedListBehaviour.GetClientIPAddress();
                    //GNetworkedListBehaviour.UpdatePlayerName(i, playerName);
                    GNetworkedListBehaviour.UpdatePlayerData(i, playerData);
                    
                    OnPlayerDataChanged?.Invoke(playerData);
                    break;
                }
            }
        }

        public LobbyPlayerData GetPlayerData()
        {
            for (int i = 0; i < GNetworkedListBehaviour.NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData = GNetworkedListBehaviour.NetworkedObjects[i];

                if (playerData.ClientId == ServerManager.LocalClientId)
                {
                    return playerData;
                }
            }

            LobbyPlayerData empty = new LobbyPlayerData();
            return empty;
        }
        
        public void Start()
        {
            // if (!ServerManager.IsClient)
            // {
            //     this.enabled = false;
            //
            //     return;
            // }

            //GNetworkedListBehaviour.OnAwakeComplete += GNetworkedListBehaviour_AwakeComplete;
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged += NetworkedObjectsOnOnListChanged;
        }

        private void GNetworkedListBehaviour_AwakeComplete()
        {
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged += NetworkedObjectsOnOnListChanged;
        }

        public void OnDestroy()
        {
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged -= NetworkedObjectsOnOnListChanged;
        }

        public void OnJoinButtonClicked()
        {
            LobbyPlayerData lobbyPlayerData = GetPlayerData();
            
            ServerManager.JoinServer(autoCreateHost: true);
        }
        
        private void NetworkedObjectsOnOnListChanged(Unity.Netcode.NetworkListEvent<LobbyPlayerData> changeEvent)
        {
            UpdateClientsList();
        }


        public void UpdateClientsList()
        {
             //if (!NetworkManager.Singleton.IsClient)
             //    return;
            
            // Destroy all client UI instances in joined clients list
            for (int idx = 0; idx < JoinedClients.transform.childCount; idx++ )
            {
                Transform child = JoinedClients.transform.GetChild(idx);
                
                GameObject.Destroy(child.gameObject);
            }

            if (GNetworkedListBehaviour.NetworkedObjects != null)
            {
                //GNetworkedListBehaviour.NetworkedObjects
                //var x = ServerManager.ConnectedClientsIds;
                // Refresh client UI with newly connected clients:
                foreach(LobbyPlayerData lobbyPlayerData in GNetworkedListBehaviour.NetworkedObjects)
                {
                    GameObject clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
                    UIClient uiClient = clientInstance.GetComponent<UIClient>();
                    uiClient.parentView = this;
                    uiClient.UpdateUI(lobbyPlayerData);
                    
                    // NetworkObject spawnedObj = ServerManager.Resolve(lobbyPlayerData.ClientId);
                    //
                    // if (spawnedObj != null)
                    // {
                    //     PlayerController playerController = spawnedObj.GetComponent<PlayerController>();
                    //
                    //     if (playerController != null)
                    //     {
                    //         GameObject clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
                    //         var uiClient = clientInstance.GetComponent<UIClient>();
                    //         uiClient.UpdateUI(playerController);
                    //
                    //     }
                    // }
                }
            }
        }
    }
}