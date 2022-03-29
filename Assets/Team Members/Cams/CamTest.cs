using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CamTest : NetworkBehaviour
{
	public ScrollRect scrollRect;
	public VisualTreeAsset visualTreeAsset;
	public NetworkList<NetworkObjectReference> NetworkedObjects = new NetworkList<NetworkObjectReference>();

    // Start is called before the first frame update
    void Start()
    {
	    
	    //spawn players
	    foreach (var player in NetworkManager.Singleton.ConnectedClients)
	    {
			// player.Key
	    }
    }

    // Update is called once per frame
    void Update()
    {
	    Debug.Log("HUH?");
    }
}
