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
            //ServerManager.JoinServerEvent += OnJoinServer;
            ServerManager.OnClientConnectedCallback += OnJoinServer;
        }
        
        public void OnDisable()
        {
            //GeralltNetworkManager.OnJoinServerEvent -= OnJoinServer;
            //ServerManager.JoinServerEvent -= OnJoinServer;
            ServerManager.OnClientConnectedCallback -= OnJoinServer;
        }

        public void OnJoinButtonClicked()
        {
            
        }
        
        private void OnJoinServer(ulong clientId)
        {
            UpdateClientsList();
        }



        public void UpdateClientsList()
        {
            // Destroy all client UI instances in joined clients list
            foreach (Transform child in JoinedClients.transform)
            {
                GameObject.Destroy(child);
            }
            
            // Refresh client UI with newly connected clients:
            foreach(NetworkObjectReference spawnedObjRef in ServerManager.NetworkedObjects)
            {
                NetworkObject spawnedObj = ServerManager.Resolve(spawnedObjRef.NetworkObjectId);

                PlayerController playerController = spawnedObj.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    GameObject clientInstance = Instantiate(UIClientPrefab, JoinedClients.transform);
                    var tmp = clientInstance.GetComponent<TextMeshPro>();
                    
                    tmp.SetText(playerController.playerName);
                }
            }
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