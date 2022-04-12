using System.Collections;
using System.Collections.Generic;
using Tom;
using Unity.Netcode;
using UnityEngine;

public class GameManager : ManagerBase<GameManager>
{
    public GameObject cameraPrefab;
    
    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += RequestSetupPlayerServerRpc;
    }
    
    [ServerRpc]
    public void RequestSetupPlayerServerRpc(ulong clientID)
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
