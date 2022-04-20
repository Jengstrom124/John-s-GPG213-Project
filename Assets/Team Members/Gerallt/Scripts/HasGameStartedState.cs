using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HasGameStartedState : ManagerBase<HasGameStartedState>
{
    public NetworkVariable<bool> hasGameStarted { get; set; } = new NetworkVariable<bool>();
}
