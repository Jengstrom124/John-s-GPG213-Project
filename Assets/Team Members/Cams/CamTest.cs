using Unity.Netcode;
using UnityEngine;

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

    public void Action()
    {
        Debug.Log("Cam ACTION");
    }

    public void Action2()
    {
    }

    public void Action3()
    {
    }
}
