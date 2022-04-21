using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{
    public class PlayerStatsUI : MonoBehaviour
    {
        public GameObject view;
        public TextMeshPro playerNameUI;
        private Transform cameraTransform;
        
        public void UpdateStats(LobbyPlayerData lobbyPlayerData)
        {
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
        }
        
        private void Awake()
        {
            if (playerNameUI == null)
            {
                playerNameUI = GetComponentInChildren<TextMeshPro>();    
            }

            cameraTransform = Camera.main.transform;
        }
        
        private void FixedUpdate()
        {
            //FollowCamera();
        }
    }
}