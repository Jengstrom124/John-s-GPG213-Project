using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

namespace Gerallt
{
    public class UILobby : MonoBehaviour
    {
        public GameObject JoinedClients;
        public GameObject UIClientPrefab;
        
        public ServerManager ServerManager;
        public GNetworkedListBehaviour GNetworkedListBehaviour;
        
        public event Action OnGameStart;
        
        public void Start()
        {
            // if (!ServerManager.IsClient)
            // {
            //     this.enabled = false;
            //
            //     return;
            // }
            
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged += NetworkedObjectsOnOnListChanged;
        }
        
        public void OnDestroy()
        {
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged -= NetworkedObjectsOnOnListChanged;
        }

        public void OnJoinButtonClicked()
        {
            
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
                    NetworkObject spawnedObj = ServerManager.Resolve(lobbyPlayerData.ClientId);

                    if (spawnedObj != null)
                    {
                        PlayerController playerController = spawnedObj.GetComponent<PlayerController>();
                    
                        if (playerController != null)
                        {
                            GameObject clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
                            var uiClient = clientInstance.GetComponent<UIClient>();
                            uiClient.UpdateUI(playerController);
                    
                        }
                    }
                }
            }
        }
    }
}