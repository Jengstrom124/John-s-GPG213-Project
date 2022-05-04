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

    #region Networked Actions and RPCs

    private Action<InputActionPhase>[] actionsArray;
    private Dictionary<ReplicatedActionType, Action<float>> actionsArrayWithParam;
    
    public enum ReplicatedActionType : int
    {
        Steer = 5,
        Accelerate = 4,
        Reverse = 3,
        Action = 2,
        Action2 = 1,
        Action3 = 0
    }

    private void Awake()
    {
	    DefaultControls defaultControls = new DefaultControls();
	    defaultControls.Enable();
	    defaultControls.InGame.Action1.performed += aContext => ReplicatedAction(ReplicatedActionType.Action, aContext.phase);
	    defaultControls.InGame.Action2.performed += aContext => ReplicatedAction(ReplicatedActionType.Action2, aContext.phase);
	    defaultControls.InGame.Action3.performed += aContext => ReplicatedAction(ReplicatedActionType.Action3, aContext.phase);
	    // defaultControls.InGame.Action1.performed += aContext => ReplicatedAction(ReplicatedActionType.Action, aContext.ReadValue<Vector2>());
    }


    private void ReplicatedAction(ReplicatedActionType actionType, Action<InputActionPhase> replicatedAction)
    {
        if (actionsArray == null)
        {
            actionsArray = new Action<InputActionPhase>[1];
        }

        int actionInd = (int) actionType;
        if (actionInd >= actionsArray.Length)
        {
            Array.Resize(ref actionsArray, actionInd + 1);
        }

        actionsArray[actionInd] = replicatedAction;
    }
    
    private void ReplicatedAction(ReplicatedActionType actionType, Action<float> replicatedAction)
    {
        if (actionsArrayWithParam == null)
        {
            actionsArrayWithParam = new Dictionary<ReplicatedActionType, Action<float>>();
        }

        if (!actionsArrayWithParam.ContainsKey(actionType))
        {
            actionsArrayWithParam.Add(actionType, replicatedAction);
        }
        else
        {
            actionsArrayWithParam[actionType] = replicatedAction;
        }
    }
    
    [ClientRpc]
    private void ReplicatedActionClientRpc(ReplicatedActionType actionType, InputActionPhase aInputActionPhase)
    {
        if (actionsArray == null) return;
        
        Action<InputActionPhase> runOnClient = actionsArray[(int) actionType];
        
        runOnClient(aInputActionPhase);
    }

    [ServerRpc]
    private void ReplicatedActionServerRpc(ReplicatedActionType actionType, InputActionPhase aInputActionPhase)
    {
        if (actionsArray == null) return;
        
        Action<InputActionPhase> replicatedAction = actionsArray[(int)actionType];
        
        replicatedAction(aInputActionPhase);
            
        ReplicatedActionClientRpc(actionType, aInputActionPhase);
    }
    
    [ClientRpc]
    private void ReplicatedActionClientRpc(ReplicatedActionType actionType, float input)
    {
        if (actionsArrayWithParam == null) return;
        
        Action<float> runOnClient = actionsArrayWithParam[actionType];
        
        runOnClient(input);
    }

    [ServerRpc]
    private void ReplicatedActionServerRpc(ReplicatedActionType actionType, float input)
    {
        if (actionsArrayWithParam == null) return;
        
        Action<float> replicatedAction = actionsArrayWithParam[actionType];
        
        replicatedAction(input);
            
        ReplicatedActionClientRpc(actionType, input);
    }

    /// <summary>
    /// The networked replicated action.
    /// </summary>
    private void ReplicatedAction(ReplicatedActionType actionType, InputActionPhase aInputActionPhase)
    {
        //Action replicatedAction = actionsArray[(int)actionType];
        
        //replicatedAction();  // Client side prediction
        if (Unity.Netcode.NetworkManager.Singleton != null &&
            (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost))
        {
            // Update the server model. The server will later update the clients.
            ReplicatedActionServerRpc(actionType, aInputActionPhase);
        }
        else
        {
            actionsArray[(int)actionType](aInputActionPhase);
        }
    }
    
    /// <summary>
    /// The networked replicated parameterised action.
    /// </summary>
    private void ReplicatedAction(ReplicatedActionType actionType, float input)
    {
        //Action<float> replicatedAction = actionsArrayWithParam[(int)actionType];
        
        //replicatedAction(input);  // Client side prediction

        if (Unity.Netcode.NetworkManager.Singleton != null &&
            (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost))
        {
            // Update the server model. The server will later update the clients.
            ReplicatedActionServerRpc(actionType, input);
        }
        else
        {
            actionsArrayWithParam[actionType](input);
        }
    }
    
    #endregion
    
    private void Start()
    {
        // Scheduled replicated actions that run on the server side and client side when requested to run.
        ReplicatedAction(ReplicatedActionType.Steer, (float input) =>
        {
            if (controlled != null)
            {
                controllable = controlled.GetComponentInChildren<IControllable>();
                controllable.Steer(input);
            }
        });
        
        ReplicatedAction(ReplicatedActionType.Accelerate, (float input) =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Accelerate(input);
        });
        
        ReplicatedAction(ReplicatedActionType.Reverse, (float input) =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Reverse(input);
        });
        
        ReplicatedAction(ReplicatedActionType.Action,  (aInputActionPhase) =>
        {
	        controllable = controlled.GetComponentInChildren<IControllable>();
	        controllable.Action(aInputActionPhase);
        });
        
        ReplicatedAction(ReplicatedActionType.Action2, (aInputActionPhase) =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Action2(aInputActionPhase);
        });
        
        ReplicatedAction(ReplicatedActionType.Action3, (aInputActionPhase) =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Action3(aInputActionPhase);
        });
    }
    
    //From here on down is taken from Cam's PlayerController - Aaron

    void Update()
    {
	    // Client side physical controls only for owned creature
	    if ((IsServer || IsClient) && !IsOwner)
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
                    ReplicatedAction(ReplicatedActionType.Steer, -1f);
                }
                else if (InputSystem.GetDevice<Keyboard>().dKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    ReplicatedAction(ReplicatedActionType.Steer, 1f);
                }
                else
                {
                    ReplicatedAction(ReplicatedActionType.Steer, 0f);
                }

                if (InputSystem.GetDevice<Keyboard>().wKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    ReplicatedAction(ReplicatedActionType.Accelerate, 1f);
                }
                else if (InputSystem.GetDevice<Keyboard>().sKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    ReplicatedAction(ReplicatedActionType.Reverse, 1f);
                }
                else
                {
	                ReplicatedAction(ReplicatedActionType.Accelerate, 0f);
	                ReplicatedAction(ReplicatedActionType.Reverse, 0f);
                }

                // if (InputSystem.GetDevice<Keyboard>().fKey.isPressed)
                // {
                //     // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                //     ReplicatedAction(ReplicatedActionType.Action);
                // }
                //
                // if (InputSystem.GetDevice<Keyboard>().eKey.isPressed)
                // {
                //     // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                //     ReplicatedAction(ReplicatedActionType.Action2);
                // }
                //
                // if (InputSystem.GetDevice<Keyboard>().qKey.isPressed)
                // {
                //     // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                //     ReplicatedAction(ReplicatedActionType.Action3);
                // }
            }
        }
    }
    
    // TODO: ServerRPC for each client control command
    // Forward on INSIDE the server with ClientRPC to sync all your ghosts on other players computers
}