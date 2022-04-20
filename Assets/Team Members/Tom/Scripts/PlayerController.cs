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

    private Action[] actionsArray;
    private Action<float>[] actionsArrayWithParam;
    
    public enum ReplicatedActionType : int
    {
        Steer = 5,
        Accelerate = 4,
        Reverse = 3,
        Action = 2,
        Action2 = 1,
        Action3 = 0
    }

    private void ReplicatedAction(ReplicatedActionType actionType, Action replicatedAction)
    {
        if (actionsArray == null)
        {
            actionsArray = new Action[1];
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
            actionsArrayWithParam = new Action<float>[1];
        }

        int actionInd = (int) actionType;
        if (actionInd >= actionsArrayWithParam.Length)
        {
            Array.Resize(ref actionsArrayWithParam, actionInd + 1);
        }

        actionsArrayWithParam[actionInd] = replicatedAction;
    }
    
    [ClientRpc]
    private void ReplicatedActionClientRpc(ReplicatedActionType actionType)
    {
        if (actionsArray == null) return;
        
        Action runOnClient = actionsArray[(int) actionType];
        
        runOnClient();
    }

    [ServerRpc]
    private void ReplicatedActionServerRpc(ReplicatedActionType actionType)
    {
        if (actionsArray == null) return;
        
        Action replicatedAction = actionsArray[(int)actionType];
        
        replicatedAction();
            
        ReplicatedActionClientRpc(actionType);
    }
    
    [ClientRpc]
    private void ReplicatedActionClientRpc(ReplicatedActionType actionType, float input)
    {
        if (actionsArrayWithParam == null) return;
        
        Action<float> runOnClient = actionsArrayWithParam[(int)actionType];
        
        runOnClient(input);
    }

    [ServerRpc]
    private void ReplicatedActionServerRpc(ReplicatedActionType actionType, float input)
    {
        if (actionsArrayWithParam == null) return;
        
        Action<float> replicatedAction = actionsArrayWithParam[(int)actionType];
        
        replicatedAction(input);
            
        ReplicatedActionClientRpc(actionType, input);
    }

    /// <summary>
    /// The networked replicated action.
    /// </summary>
    private void ReplicatedAction(ReplicatedActionType actionType)
    {
        //Action replicatedAction = actionsArray[(int)actionType];
        
        //replicatedAction();  // Client side prediction

        if (NetworkManager.Singleton.IsConnectedClient)
        {
            // Update the server model. The server will later update the clients.
            ReplicatedActionServerRpc(actionType);
        }
        else
        {
            actionsArray[(int)actionType]();
        }
    }
    
    /// <summary>
    /// The networked replicated parameterised action.
    /// </summary>
    private void ReplicatedAction(ReplicatedActionType actionType, float input)
    {
        //Action<float> replicatedAction = actionsArrayWithParam[(int)actionType];
        
        //replicatedAction(input);  // Client side prediction

        if (NetworkManager.Singleton.IsConnectedClient)
        {
            // Update the server model. The server will later update the clients.
            ReplicatedActionServerRpc(actionType, input);
        }
        else
        {
            actionsArrayWithParam[(int)actionType](input);
        }
    }
    
    #endregion
    
    private void Start()
    {
        // Scheduled replicated actions that run on the server side and client side when requested to run.
        ReplicatedAction(ReplicatedActionType.Steer, (float input) =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Steer(input);
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
        
        ReplicatedAction(ReplicatedActionType.Action, () =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Action();
        });
        
        ReplicatedAction(ReplicatedActionType.Action2, () =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Action2();
        });
        
        ReplicatedAction(ReplicatedActionType.Action3, () =>
        {
            controllable = controlled.GetComponentInChildren<IControllable>();
            controllable.Action3();
        });
    }
    
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

                if (InputSystem.GetDevice<Keyboard>().sKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    ReplicatedAction(ReplicatedActionType.Reverse, 1f);
                }

                if (InputSystem.GetDevice<Keyboard>().fKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    ReplicatedAction(ReplicatedActionType.Action);
                }

                if (InputSystem.GetDevice<Keyboard>().eKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    ReplicatedAction(ReplicatedActionType.Action2);
                }

                if (InputSystem.GetDevice<Keyboard>().qKey.isPressed)
                {
                    // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                    ReplicatedAction(ReplicatedActionType.Action3);
                }
            }
        }
    }
    
    // TODO: ServerRPC for each client control command
    // Forward on INSIDE the server with ClientRPC to sync all your ghosts on other players computers
}