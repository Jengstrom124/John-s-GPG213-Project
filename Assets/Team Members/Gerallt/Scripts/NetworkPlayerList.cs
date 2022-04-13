using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{
    public class NetworkPlayerList : NetworkBehaviour
    {
        public ServerManager ServerManager;
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

        private void Start()
        {

        }
        
        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate connectionApprovedCallback)
        {
            Debug.Log("Client connection approval " + clientId.ToString());
        }

        public override void OnNetworkSpawn()
        {
            // {
            //     ServerManager.ConnectionApprovalCallback += ApprovalCheck; 
            //     ServerManager.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
            //     ServerManager.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
            // }
            
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
            lobbyPlayerData.ClientIPAddress = GetClientIPAddress();
            
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

        public LobbyPlayerData GetPlayerData()
        {
            for (int i = 0; i < NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData = NetworkedObjects[i];

                if (playerData.ClientId == ServerManager.LocalClientId)
                {
                    return playerData;
                }
            }

            LobbyPlayerData empty = new LobbyPlayerData();
            empty.PlayerName = "Unknown";
            return empty;
        }
        
        public LobbyPlayerData GetPlayerDataByClientId(ulong clientId)
        {
            for (int i = 0; i < NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData = NetworkedObjects[i];

                if (playerData.ClientId == clientId)
                {
                    return playerData;
                }
            }

            LobbyPlayerData empty = new LobbyPlayerData();
            empty.PlayerName = "Unknown";
            return empty;
        }
        
        public void UpdatePlayerName(int i, string newPlayerName)
        {
            if (ServerManager.IsClient)
            {
                UpdatePlayerNameServerRpc(i, newPlayerName);    
            }
        }

        public void UpdatePlayerData(int i, LobbyPlayerData newData)
        {
            if (ServerManager.IsClient)
            {
                UpdatePlayerDataServerRpc(i, newData);    
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
        
        [ServerRpc(RequireOwnership = false)]
        void UpdatePlayerDataServerRpc(int i, LobbyPlayerData newData)
        {
            if (i < 0 || i >= NetworkedObjects.Count)
                return;
            
            NetworkedObjects[i] = newData;
        }
        
        public static string GetClientIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    return ip.ToString();
                }
            }
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6) // IPv6
                {
                    return ip.ToString();
                }
            }

            return string.Empty;
        }
    }
}
