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
        
        public void Awake()
        {
            //ServerManager.JoinServerEvent += OnJoinServer;
            //ServerManager.OnClientConnectedCallback += OnJoinServer;
            GNetworkedListBehaviour.NetworkedObjects.OnListChanged += NetworkedObjectsOnOnListChanged;
        }

        public void OnDisable()
        {
            //GeralltNetworkManager.OnJoinServerEvent -= OnJoinServer;
            //ServerManager.JoinServerEvent -= OnJoinServer;
            //ServerManager.OnClientConnectedCallback -= OnJoinServer;
            //GNetworkedListBehaviour.NetworkedObjects.OnListChanged -= NetworkedObjectsOnOnListChanged;
        }

        public void OnJoinButtonClicked()
        {
            
        }
        
        private void OnJoinServer(ulong clientId)
        {
            UpdateClientsList();
        }

        private void NetworkedObjectsOnOnListChanged(Unity.Netcode.NetworkListEvent<ulong> changeEvent)
        {
            UpdateClientsList();
        }


        public void UpdateClientsList()
        {
            // if (!NetworkManager.Singleton.IsClient)
            //     return;
            
            // Destroy all client UI instances in joined clients list
            foreach (Transform child in JoinedClients.transform)
            {
                GameObject.Destroy(child);
            }

            if (GNetworkedListBehaviour.NetworkedObjects != null)
            {
                // Refresh client UI with newly connected clients:
                foreach(ulong clientId in GNetworkedListBehaviour.NetworkedObjects)
                {
                    NetworkObject spawnedObj = ServerManager.Resolve(clientId);

                    PlayerController playerController = spawnedObj.GetComponent<PlayerController>();

                    if (playerController != null)
                    {
                        GameObject clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
                        var tmp = clientInstance.GetComponent<UIClient>();
                        tmp.UpdateUI(playerController);

                    }
                }
            }
        }
    }
}