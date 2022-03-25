using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{
    public class GNetworkedListBehaviour : NetworkBehaviour
    {
        public ServerManager ServerManager;
        //public NetworkList<NetworkObjectReference> NetworkedObjects;// = new NetworkList<NetworkObjectReference>();
        //[SerializeField] public NetworkList<ulong> NetworkedObjects = new NetworkList<ulong>(NetworkVariableReadPermission.Everyone, new List<ulong>());
        public NetworkList<LobbyPlayerData> NetworkedObjects;
        
        public void Awake()
        {
            //NetworkedObjects = new NetworkList<LobbyPlayerData>(NetworkVariableReadPermission.Everyone, new List<LobbyPlayerData>());
            NetworkedObjects = new NetworkList<LobbyPlayerData>();
            
            //if (ServerManager.IsServer)
            {
                ServerManager.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
                ServerManager.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
            }
        }
        
        // public override void OnNetworkSpawn()
        // {
        //     if (!IsClient)
        //     {
        //         enabled = false;
        //     }
        //     else
        //     {
        //         ServerManager.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
        //         ServerManager.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
        //     }
        // }
        //
        // public override void OnNetworkDespawn()
        // {
        //     ServerManager.OnClientConnectedCallback -= ServerManager_OnOnClientConnectedCallback;
        //     ServerManager.OnClientDisconnectCallback -= ServerManager_OnOnClientDisconnectCallback;
        // }
        
        public void OnDisable()
        {
            //ServerManager.OnClientConnectedCallback -= ServerManager_OnOnClientConnectedCallback;
            //ServerManager.OnClientDisconnectCallback -= ServerManager_OnOnClientDisconnectCallback;
        }

        private void ServerManager_OnOnClientConnectedCallback(ulong clientId)
        {
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            lobbyPlayerData.ClientId = clientId;
            
            NetworkedObjects.Add(lobbyPlayerData);

            //UpdateOnConnectedClientRpc(clientId);
        }
        
        private void ServerManager_OnOnClientDisconnectCallback(ulong clientId)
        {
            // Remove LobbyPlayerData object matching clientId from NetworkedObjects list 
            for (int i = 0; i < NetworkedObjects.Count; i++)
            {
                LobbyPlayerData item = NetworkedObjects[i];

                if (item.ClientId == clientId)
                {
                    NetworkedObjects.Remove(item);
                    break;
                }
            }
        }

        [ClientRpc]
        void UpdateOnConnectedClientRpc(ulong clientId)
        {
            // LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            // lobbyPlayerData.ClientId = clientId;
            //
            // NetworkedObjects.Add(lobbyPlayerData);
        }
    }
}
