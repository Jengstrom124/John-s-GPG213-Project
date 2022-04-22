using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{
    public class NetworkPlayerList : ManagerBase<NetworkPlayerList>
    {
        public NetworkList<LobbyPlayerData> NetworkedObjects;

        public event Action<ulong, LobbyPlayerData> OnPlayerDataChanged;

        public override void Awake()
        {
            base.Awake();
            
            NetworkedObjects = new NetworkList<LobbyPlayerData>();

            //ServerManager.ConnectionApprovalCallback += ApprovalCheck; 
            ServerManager.Singleton.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
            ServerManager.Singleton.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
        }
        
        private void ServerManager_OnOnClientConnectedCallback(ulong clientId)
        {
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
            lobbyPlayerData.ClientId = clientId;
            lobbyPlayerData.ClientIPAddress = GetClientIPAddress();
            
            NetworkedObjects.Add(lobbyPlayerData); 
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

        public bool HasJoined(ulong clientId)
        {
            bool joined = false;
            for (int i = 0; i < NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData = NetworkedObjects[i];

                if (playerData.ClientId == clientId)
                {
                    joined = true;
                    break;
                }
            }

            return joined;
        }
        
        public LobbyPlayerData GetPlayerData()
        {
            for (int i = 0; i < NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData = NetworkedObjects[i];

                if (playerData.ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    return playerData;
                }
            }

            LobbyPlayerData empty = new LobbyPlayerData();
            empty.ClientId = NetworkManager.Singleton.LocalClientId;
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
            if (ServerManager.Singleton.IsClient)
            {
                UpdatePlayerNameServerRpc(i, newPlayerName);    
            }
        }

        public void UpdatePlayerData(int i, LobbyPlayerData newData)
        {
            if (ServerManager.Singleton.IsClient)
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
            
            RaisePlayerDataChangedClientRpc(data);
        }
        
        [ServerRpc(RequireOwnership = false)]
        void UpdatePlayerDataServerRpc(int i, LobbyPlayerData newData)
        {
            if (i < 0 || i >= NetworkedObjects.Count)
                return;
            
            NetworkedObjects[i] = newData;
            
            // Tell the clients the updated data:
            //UpdatePlayerDataClientRpc(i, newData);

            RaisePlayerDataChangedClientRpc(newData);
        }
        
        [ClientRpc]
        void UpdatePlayerDataClientRpc(int i, LobbyPlayerData newData)
        {
            if (i < 0 || i >= NetworkedObjects.Count)
                return;
            
            NetworkedObjects[i] = newData;
        }
        
        [ClientRpc]
        void RaisePlayerDataChangedClientRpc(LobbyPlayerData newData)
        {
            OnPlayerDataChanged?.Invoke(newData.ClientId, newData);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void UpdatePlayerDataServerRpc(LobbyPlayerData newData)
        {
            for (int i = 0; i < NetworkedObjects.Count; i++)
            {
                LobbyPlayerData playerData = NetworkedObjects[i];

                if (playerData.ClientId == newData.ClientId)
                {
                    NetworkedObjects[i] = newData;
                    
                    RaisePlayerDataChangedClientRpc(newData);
                }
            }
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
