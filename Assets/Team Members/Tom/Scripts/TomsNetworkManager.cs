using System.Collections;
using System.Collections.Generic;
using Tom;
using Unity.Netcode;
using UnityEngine;

public class TomsNetworkManager : NetworkManager
{
    public GameObject cameraPrefab;
    
    public void Awake()
    {
        OnClientConnectedCallback += SetupPlayer;
    }

    public void SetupPlayer(ulong clientID)
    {
        if (clientID == LocalClientId)
        {
            CameraFollow newCamera = Instantiate(cameraPrefab).GetComponent<CameraFollow>();
            newCamera.target = LocalClient.PlayerObject.transform;
            newCamera.offset = new Vector3(0f, 15f, 0f); // HACK
        }
    }
}
