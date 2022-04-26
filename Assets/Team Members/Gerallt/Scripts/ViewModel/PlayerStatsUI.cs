using System;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{
    public class PlayerStatsUI : MonoBehaviour
    {
        public GameObject view;
        public TextMeshPro playerNameUI;
        public Vector3 offset = new Vector3(0, 0, -4);
        
        private Transform cameraTransform;
        private LobbyPlayerData playerDataCache;
        private ulong clientId;
        
        public void UpdateStats(LobbyPlayerData lobbyPlayerData)
        {
            playerDataCache = lobbyPlayerData;
            clientId = lobbyPlayerData.ClientId;

            if (string.IsNullOrEmpty(lobbyPlayerData.PlayerName))
            {
                playerNameUI.text = lobbyPlayerData.ClientIPAddress;
            }
            else
            {
                playerNameUI.text = lobbyPlayerData.PlayerName;
            }

            FollowCamera();
        }

        private void FollowCamera()
        {
            transform.LookAt(cameraTransform.position);
            transform.Rotate(new Vector3(0,1, 0), -180); // HACK: Had to rotate by 180 
            transform.rotation = quaternion.identity; // HACK
        }
        
        private void Awake()
        {
            if (playerNameUI == null)
            {
                playerNameUI = GetComponentInChildren<TextMeshPro>();    
            }

            cameraTransform = Camera.main.transform;
        }

        private void Start()
        {
            NetworkPlayerList.Instance.OnPlayerDataChanged += OnPlayerDataChanged;
        }

        private void OnDestroy()
        {
            NetworkPlayerList.Instance.OnPlayerDataChanged -= OnPlayerDataChanged;
        }

        private void OnPlayerDataChanged(ulong playerId, LobbyPlayerData lobbyPlayerData)
        {
            if (clientId == playerId)
            {
                UpdateStats(lobbyPlayerData);
            }
        }

        private void FixedUpdate()
        {
            //FollowCamera();
        }
    }
}