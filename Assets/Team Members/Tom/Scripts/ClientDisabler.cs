using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientDisabler : NetworkBehaviour
{
    public List<MonoBehaviour> componentsToDisable = new List<MonoBehaviour>();
    
    public void OnConnectedToServer()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += DisableComponents;
    }

    public void DisableComponents(ulong client)
    {
        if (client == NetworkManager.Singleton.LocalClientId)
        {
            foreach (MonoBehaviour component in componentsToDisable)
            {
                component.enabled = false;
            }
        }
    }
}
