using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OllieFish : FishBase, IRTS, IEdible
{
    public PathTracker pathTracker;
    public IStateBase flock;
    public IStateBase pathFollow;
    

    // Start is called before the first frame update
    void Start()
    {
        //this shit was all from inheriting OllieVehicleBase - when I thought the fish needed to be controllable but IRTS replaced this
        // rb = GetComponentInChildren<Rigidbody>();
        // capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        // forwardSpeed = 40;
        // turnSpeed = 20;
        // car = this.gameObject;
    }

    private void OnEnable()
    {
        pathTracker.destinationReachedEvent += Deselected;
    }

    private void OnDisable()
    {
        pathTracker.destinationReachedEvent -= Deselected;
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
        pathTracker.GetPathToDestination(position);
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

    public void GotShatOut(IPredator shatOutBy)
    {
        throw new NotImplementedException();
    }

    #endregion
    
}
