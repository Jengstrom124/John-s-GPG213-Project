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
    private void SteerClientRpc(float input, PlayerTransform playerTransform)
    {
        ApplyTransform(playerTransform);
            
        controllable.Steer(input);
    }

    [ServerRpc]
    private void SteerServerRpc(float input)
    {
        controllable.Steer(input);
            
        SteerClientRpc(input, PackageTransform());
    }
    
    [ClientRpc]
    private void AccelerateClientRpc(float input, PlayerTransform playerTransform)
    {
        ApplyTransform(playerTransform);
            
        controllable.Accelerate(input);
    }

    [ServerRpc]
    private void AccelerateServerRpc(float input)
    {
        controllable.Accelerate(input);
            
        AccelerateClientRpc(input, PackageTransform());
    }
    
    [ClientRpc]
    private void ReverseClientRpc(float input, PlayerTransform playerTransform)
    {
        ApplyTransform(playerTransform);
            
        controllable.Reverse(input);
    }

    [ServerRpc]
    private void ReverseServerRpc(float input)
    {
        controllable.Reverse(input);
            
        ReverseClientRpc(input, PackageTransform());
    }
    
    [ClientRpc]
    private void ActionClientRpc(PlayerTransform playerTransform)
    {
        ApplyTransform(playerTransform);
            
        controllable.Action();
    }

    [ServerRpc]
    private void ActionServerRpc()
    {
        controllable.Action();
            
        ActionClientRpc(PackageTransform());
    }
    
    [ClientRpc]
    private void Action2ClientRpc(PlayerTransform playerTransform)
    {
        ApplyTransform(playerTransform);
            
        controllable.Action2();
    }

    [ServerRpc]
    private void Action2ServerRpc()
    {
        controllable.Action2();
            
        Action2ClientRpc(PackageTransform());
    }

    [ClientRpc]
    private void Action3ClientRpc(PlayerTransform playerTransform)
    {
        ApplyTransform(playerTransform);
            
        controllable.Action3();
    }

    [ServerRpc]
    private void Action3ServerRpc()
    {
        controllable.Action3();
            
        Action3ClientRpc(PackageTransform());
    }
    
    #endregion
    
    private void ApplyTransform(PlayerTransform playerTransform)
    {
        Transform _transform = controlled.transform;
        Rigidbody rb = controlled.GetComponent<Rigidbody>();
        
        _transform.position = playerTransform.position;
        _transform.rotation = playerTransform.rotation;
        
        if (rb != null)
        {
            rb.velocity = playerTransform.velocity;
            rb.angularVelocity = playerTransform.angularVelocity;
        }
    }
    
    private PlayerTransform PackageTransform()
    {
        Transform _transform = controlled.transform;
        Rigidbody rb = controlled.GetComponent<Rigidbody>();

        Vector3 velocity = Vector3.zero;
        Vector3 angularVelocity = Vector3.zero;

        if (rb != null)
        {
            velocity = rb.velocity;
            angularVelocity = rb.angularVelocity;
        }
            
        PlayerTransform playerTransform = new PlayerTransform(
            clientInfo.Value.ClientId,
            _transform.position,
            _transform.rotation,
            velocity,
            angularVelocity
        );

        return playerTransform;
    }

    /// <summary>
    /// The networked version of Steer(float).
    /// </summary>
    private void Steer(float input)
    {
        controllable.Steer(input);

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
        controllable.Accelerate(input);

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
        controllable.Reverse(input);

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
        controllable.Action();

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
        controllable.Action2();

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
        controllable.Action3();

        if (isNetworked)
        {
            // Update the server model. The server will later update the clients.
            Action3ServerRpc();
        }
    }
    
    #endregion

    //From here on down is taken from Cam's PlayerController - Aaron
    
    private void Start()
    {
        if (controlled != null)
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
        }
    }
    
    void Update()
    {
	    // Client side physical controls only for owned creature
	    if (!IsOwner)
		    return;
	    
        // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        if (controlled != null)
        {
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