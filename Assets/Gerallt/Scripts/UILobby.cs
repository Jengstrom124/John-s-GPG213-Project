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
        //public GeralltNetworkManager GeralltNetworkManager;
        public ServerManager ServerManager;
        
        public void OnEnable()
        {
            //GeralltNetworkManager.OnJoinServerEvent += OnJoinServer;
            ServerManager.JoinServerEvent += OnJoinServer;
        }
        
        public void OnDisable()
        {
            //GeralltNetworkManager.OnJoinServerEvent -= OnJoinServer;
            ServerManager.JoinServerEvent -= OnJoinServer;
        }

        public void OnJoinButtonClicked()
        {
            
        }
        
        private void OnJoinServer(int clientId)
        {
            UpdateClientsList();
        }



        public void UpdateClientsList()
        {
            // // Destroy all client UI instances in joined clients list
            // foreach (Transform child in JoinedClients.transform)
            // {
            //     GameObject.Destroy(child);
            // }
            //
            // // Refresh client UI with newly connected clients:
            // //foreach (var client in NetworkManager.Singleton.ConnectedClients)
            // foreach (var client in GeralltNetworkManager.NetworkedClientIds)
            // {
            //     var clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
            //     var tmp = clientInstance.GetComponent<TextMeshPro>();
            //     //tmp.SetText(client.Value.ClientId.ToString());
            //     tmp.SetText(client.ToString());
            // }
        }
        
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}