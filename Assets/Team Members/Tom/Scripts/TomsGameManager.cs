using System.Collections;
using System.Collections.Generic;
using Tom;
using Unity.Netcode;
using UnityEngine;

public class TomsGameManager : MonoBehaviour
{
    public GameObject cameraPrefab;
    
    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SetupPlayer;
    }

    public void SetupPlayer(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            CameraFollow newCamera = Instantiate(cameraPrefab).GetComponent<CameraFollow>();
            newCamera.target = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
            newCamera.offset = new Vector3(0f, 15f, 0f); // HACK
            newCamera.GetComponent<NetworkObject>().Spawn();
        }
    }
}
