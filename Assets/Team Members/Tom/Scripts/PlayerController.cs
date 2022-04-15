using System;
using System.Collections;
using System.Collections.Generic;
using Gerallt;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public bool isNetworked;

    //From here on down is taken from Cam's PlayerController - Aaron
    public GameObject controlled;

    [ClientRpc]
    private void SteerClientRpc(float input, PlayerTransform playerTransform)
    {
        IControllable controllable = controlled.GetComponentInChildren<IControllable>();

        if (controllable != null)
        {
            ApplyTransform(playerTransform);
            
            controllable.Steer(input);
        }
    }

    [ServerRpc]
    private void SteerServerRpc(float input)
    {
        IControllable controllable = controlled.GetComponentInChildren<IControllable>();

        if (controllable != null)
        {
            controllable.Steer(input);
            
            SteerClientRpc(input, PackageTransform());
        }
    }
    
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
        IControllable controllable = controlled.GetComponentInChildren<IControllable>();

        if (controllable != null)
        {
            controllable.Steer(input);

            if (isNetworked)
            {
                // Update the server model. The server will later update the clients.
                SteerServerRpc(input);
            }
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
            IControllable controllable = controlled.GetComponentInChildren<IControllable>();

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
                    controllable.Accelerate(1f);
                }

                if (InputSystem.GetDevice<Keyboard>().sKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    controllable.Reverse(1f);
                }

                if (InputSystem.GetDevice<Keyboard>().fKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    controllable.Action();
                }

                if (InputSystem.GetDevice<Keyboard>().eKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    controllable.Action2();
                }

                if (InputSystem.GetDevice<Keyboard>().qKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    controllable.Action3();
                }
            }
        }
    }
    
    // TODO: ServerRPC for each client control command
    // Forward on INSIDE the server with ClientRPC to sync all your ghosts on other players computers
}