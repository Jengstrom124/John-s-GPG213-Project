using System.Collections;
using System.Collections.Generic;
using Tom;
using Unity.Netcode;
using UnityEngine;

public class GameManager : ManagerBase<GameManager>
{
    public List<string> levels = new List<string>();
    
    public GameObject cameraPrefab;

    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SetupPlayer;
    }

    public void SetupPlayer(ulong clientID)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GetComponent<NetworkObject>().Spawn();
            SetupCameraClientRpc(clientID);
        }
    }

    [ClientRpc]
    public void SetupCameraClientRpc(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            CameraFollow newCamera = Instantiate(cameraPrefab).GetComponent<CameraFollow>();
            newCamera.target = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.transform;
            newCamera.offset = new Vector3(0f, 15f, 0f); // HACK: Hard-coded, get this value from shark's zoom level
            newCamera.GetComponent<NetworkObject>().Spawn();
        }
    }
}