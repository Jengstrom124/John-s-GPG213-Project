using System;
using System.Collections;
using System.Collections.Generic;
using Gerallt;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    public GameObject selectedCharacter;

    public NetworkVariable<LobbyPlayerData> clientInfo;

    public string playerName
    {
        get => clientInfo.Value.PlayerName;
        set
        {
            LobbyPlayerData playerData = clientInfo.Value;
            playerData.PlayerName = value;
            clientInfo.Value = playerData;
            
            // Update the internal list with the new player data
            NetworkPlayerList.Instance.UpdatePlayerDataServerRpc(playerData);
        }
    }

    public NetworkVariable<Color> playerColour;
    public bool isNetworked = true;
    public GameObject controlled;
    
    private IControllable controllable;

    #region Networked Behaviours 
    
    #region RPCs
    
    [ClientRpc]
    private void SteerClientRpc(float input)
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Steer(input);
    }

    [ServerRpc]
    private void SteerServerRpc(float input)
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Steer(input);
            
        SteerClientRpc(input);
    }
    
    [ClientRpc]
    private void AccelerateClientRpc(float input)
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Accelerate(input);
    }

    [ServerRpc]
    private void AccelerateServerRpc(float input)
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Accelerate(input);
            
        AccelerateClientRpc(input);
    }
    
    [ClientRpc]
    private void ReverseClientRpc(float input)
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Reverse(input);
    }

    [ServerRpc]
    private void ReverseServerRpc(float input)
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Reverse(input);
            
        ReverseClientRpc(input);
    }
    
    [ClientRpc]
    private void ActionClientRpc()
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Action();
    }

    [ServerRpc]
    private void ActionServerRpc()
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Action();
            
        ActionClientRpc();
    }
    
    [ClientRpc]
    private void Action2ClientRpc()
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Action2();
    }

    [ServerRpc]
    private void Action2ServerRpc()
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Action2();
            
        Action2ClientRpc();
    }

    [ClientRpc]
    private void Action3ClientRpc()
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Action3();
    }

    [ServerRpc]
    private void Action3ServerRpc()
    {
        controllable = controlled.GetComponentInChildren<IControllable>();
        controllable.Action3();
            
        Action3ClientRpc();
    }
    
    #endregion
    
    // private void ApplyTransform(PlayerTransform playerTransform)
    // {
    //     Transform _transform = controlled.transform;
    //     Rigidbody rb = controlled.GetComponent<Rigidbody>();
    //     
    //     _transform.position = playerTransform.position;
    //     _transform.rotation = playerTransform.rotation;
    //     
    //     if (rb != null)
    //     {
    //         rb.velocity = playerTransform.velocity;
    //         rb.angularVelocity = playerTransform.angularVelocity;
    //     }
    // }
    //
    // private PlayerTransform PackageTransform()
    // {
    //     Transform _transform = controlled.transform;
    //     Rigidbody rb = controlled.GetComponent<Rigidbody>();
    //
    //     Vector3 velocity = Vector3.zero;
    //     Vector3 angularVelocity = Vector3.zero;
    //
    //     if (rb != null)
    //     {
    //         velocity = rb.velocity;
    //         angularVelocity = rb.angularVelocity;
    //     }
    //         
    //     PlayerTransform playerTransform = new PlayerTransform(
    //         clientInfo.Value.ClientId,
    //         _transform.position,
    //         _transform.rotation,
    //         velocity,
    //         angularVelocity
    //     );
    //
    //     return playerTransform;
    // }

    /// <summary>
    /// The networked version of Steer(float).
    /// </summary>
    private void Steer(float input)
    {
        //controllable.Steer(input); // Client side prediction

        if (isNetworked)
        {
            // Update the server model. The server will later update the clients.
            SteerServerRpc(input);
        }
    }
    
    /// <summary>
    /// The networked version of Accelerate(float).
    /// </summary>
    private void Accelerate(float input)
    {
        //controllable.Accelerate(input); // Client side prediction

        if (isNetworked)
        {
            // Update the server model. The server will later update the clients.
            AccelerateServerRpc(input);
        }
    }
    
    /// <summary>
    /// The networked version of Reverse(float).
    /// </summary>
    private void Reverse(float input)
    {
        //controllable.Reverse(input);  // Client side prediction

        if (isNetworked)
        {
            // Update the server model. The server will later update the clients.
            ReverseServerRpc(input);
        }
    }
    
    /// <summary>
    /// The networked version of Action().
    /// </summary>
    private void Action()
    {
        //controllable.Action();  // Client side prediction

        if (isNetworked)
        {
            // Update the server model. The server will later update the clients.
            ActionServerRpc();
        }
    }
    
    /// <summary>
    /// The networked version of Action2().
    /// </summary>
    private void Action2()
    {
        //controllable.Action2();  // Client side prediction

        if (isNetworked)
        {
            // Update the server model. The server will later update the clients.
            Action2ServerRpc();
        }
    }
    
    /// <summary>
    /// The networked version of Action().
    /// </summary>
    private void Action3()
    {
        //controllable.Action3();  // Client side prediction

        if (isNetworked)
        {
            // Update the server model. The server will later update the clients.
            Action3ServerRpc();
        }
    }
    
    #endregion

    //From here on down is taken from Cam's PlayerController - Aaron

    void Update()
    {
	    // Client side physical controls only for owned creature
	    if (!IsOwner)
		    return;
	    
        // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        if (controlled != null)
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            
            if (controllable != null)
            {
                if (InputSystem.GetDevice<Keyboard>().aKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    Steer(-1f);
                }
                else if (InputSystem.GetDevice<Keyboard>().dKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    Steer(1f);
                }
                else
                {
                    Steer(0f);
                }

                if (InputSystem.GetDevice<Keyboard>().wKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    Accelerate(1f);
                }

                if (InputSystem.GetDevice<Keyboard>().sKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    Reverse(1f);
                }

                if (InputSystem.GetDevice<Keyboard>().fKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    Action();
                }

                if (InputSystem.GetDevice<Keyboard>().eKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    Action2();
                }

                if (InputSystem.GetDevice<Keyboard>().qKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    Action3();
                }
            }
        }
    }
    
    // TODO: ServerRPC for each client control command
    // Forward on INSIDE the server with ClientRPC to sync all your ghosts on other players computers
}