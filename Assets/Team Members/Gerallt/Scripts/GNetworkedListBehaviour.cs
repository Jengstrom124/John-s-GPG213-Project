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

        public Action OnAwakeComplete;
        
        public void Awake()
        {
            //NetworkedObjects = new NetworkList<LobbyPlayerData>(NetworkVariableReadPermission.Everyone, new List<LobbyPlayerData>());
            NetworkedObjects = new NetworkList<LobbyPlayerData>();
            
            //if (ServerManager.IsServer)
            {
                ServerManager.ConnectionApprovalCallback += ApprovalCheck; 
                ServerManager.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
                ServerManager.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
            }

            if (OnAwakeComplete != null)
            {
                OnAwakeComplete();
            }
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate connectionApprovedCallback)
        {
            Debug.Log("Client connection approval " + clientId.ToString());
        }

        public override void OnNetworkSpawn()
        {
            // if (!IsClient || !IsHost)
            // {
            //     //enabled = false;
            // }
            // else
            // {
            //     ServerManager.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
            //     ServerManager.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
            // }
            
            //ServerManager.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
            //ServerManager.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
        }
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
            
            // if (NetworkedObjects.Count == 0)
            // {
            //     NetworkedObjects.Add(lobbyPlayerData);
            // }
            // else
            // {
            //     NetworkedObjects.Insert((int)clientId, lobbyPlayerData);
            // }
            
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

        public void UpdatePlayerName(int i, string newPlayerName)
        {

            if (ServerManager.IsClient)
            {
                UpdatePlayerNameServerRpc(i, newPlayerName);    
            }
            
        }
        
        [ServerRpc(RequireOwnership = false)]
        void UpdatePlayerNameServerRpc(int i, string newPlayerName)
        {
            if (i < 0 || i >= NetworkedObjects.Count)
                return;
            
            LobbyPlayerData data = NetworkedObjects[i];
                
            data.PlayerName = newPlayerName;

            NetworkedObjects[i] = data;
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
