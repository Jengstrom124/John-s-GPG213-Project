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
        [SerializeField] public NetworkList<ulong> NetworkedObjects = new NetworkList<ulong>();
        public void Awake()
        {
            //if(NetworkedObjects!= null) NetworkedObjects.Dispose();
            
            //NetworkedObjects = new NetworkList<NetworkObjectReference>();
            
            //NetworkedObjects = new GNetworkList<ulong>(NetworkVariableReadPermission.Everyone, new List<ulong>());
            
            ServerManager.OnClientConnectedCallback += ServerManager_OnOnClientConnectedCallback;
            ServerManager.OnClientDisconnectCallback += ServerManager_OnOnClientDisconnectCallback;
        }

        public void OnDisable()
        {
            //ServerManager.OnClientConnectedCallback -= ServerManager_OnOnClientConnectedCallback;
            //ServerManager.OnClientDisconnectCallback -= ServerManager_OnOnClientDisconnectCallback;
        }

        private void ServerManager_OnOnClientConnectedCallback(ulong clientId)
        {
            NetworkedObjects.Add(clientId);
        }
        
        private void ServerManager_OnOnClientDisconnectCallback(ulong clientId)
        {
            NetworkedObjects.Remove(clientId);
        }
    }
}
