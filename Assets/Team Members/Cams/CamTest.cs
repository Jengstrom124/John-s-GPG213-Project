using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamTest : NetworkBehaviour, IControllable
{
    public void Steer(float input)
    {
        
    }

    public void Accelerate(float input)
    {
    }

    public void Reverse(float input)
    {
    }

    public void Action(InputActionPhase aActionPhase)
    {
        Debug.Log("Cam ACTION");
    }

    public void Action2(InputActionPhase aActionPhase)
    {
    }

    public void Action3(InputActionPhase aActionPhase)
    {
    }
}
