using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{
    public class GNetworkState : NetworkBehaviour
    {
        [SerializeField] private GameObject gNetworkedListBehaviourPrefab;
        
        private GameObject go;
        
        public void Init(ServerManager networkManager)
        {
            go = Instantiate(gNetworkedListBehaviourPrefab);
            go.GetComponent<NetworkPlayerList>().ServerManager = networkManager;
        }

        public void Spawn()
        {
            go.GetComponent<NetworkObject>().Spawn();
        }
    }
}
