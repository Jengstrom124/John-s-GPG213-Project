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

    public LobbyPlayerData? clientInfo;

    public string playerName;

    public NetworkVariable<Color> playerColour;


    //From here on down is taken from Cam's PlayerController - Aaron
    public GameObject controlledThing;

    void Update()
    {
	    // Client side physical controls only for owned creature
	    if (!IsOwner)
		    return;
	    
        // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        if (controlledThing != null)
        {
            IControllable controllable = controlledThing.GetComponentInChildren<IControllable>();

            if (controllable != null)
            {
                controllable.Steer(0f);
                if (InputSystem.GetDevice<Keyboard>().aKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    controllable.Steer(-1f);
                }

                if (InputSystem.GetDevice<Keyboard>().dKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    controllable.Steer(1f);
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