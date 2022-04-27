using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OllieFish : OllieVehicleBase, IRTS, IEdible
{
    public IStateBase flock;
    public IStateBase pathFollow;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        forwardSpeed = 40;
        turnSpeed = 20;
        car = this.gameObject;
    }

    #region IRTS Interface

    public void Selected()
    {
        flock.Exit();
        pathFollow.Enter();
    }

    public void Deselected()
    {
        flock.Enter();
        pathFollow.Exit();
    }

    public void SetDestination(Vector3 position)
    {
        pathFollow.Execute(); //???? not sure what goes here?
    }

    #endregion

    #region IEdible Interface

    public void GetEaten(IPredator eatenBy)
    {
        
    }

    public EdibleInfo GetInfo()
    {
        return new EdibleInfo();
    }

    #endregion
    
}
