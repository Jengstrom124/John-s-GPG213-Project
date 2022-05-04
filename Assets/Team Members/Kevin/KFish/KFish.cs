using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using Luke;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Flock = Kevin.Flock;
using Vector3 = UnityEngine.Vector3;

public class KFish : FishBase, IControllable, IReactsToWater, IStateBase, IRTS, IEdible
{
    //State Base
    public IStateBase flock;
    public IStateBase pathFollow;

    public List<IStateBase> stateManager; 
    
    public bool flocking;
    public bool following;
    
    
    //Fish Movement Variables
    public Rigidbody fRb;
    public Transform tailObjectTransform;

    public int foodValue;
    public float acceleration = 10f;
    public float currentSteeringAngle;
    public float steeringMax = 30f;
    public float reduction = 0.5f;
    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;

    void Start()
    {
        fRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 tailPosition = tailObjectTransform.position;
        localVelocity = transform.InverseTransformDirection(fRb.velocity);
        tailLocalVelocity = tailObjectTransform.InverseTransformDirection(fRb.GetPointVelocity(tailPosition));
        
        fRb.AddRelativeForce(fRb.mass*new Vector3 (-localVelocity.x, 0, 0));
        
        fRb.AddForceAtPosition(fRb.mass*tailObjectTransform.
            TransformDirection(new Vector3 (-tailLocalVelocity.x, 0, 0)), tailPosition);

        if (localVelocity.z > 0)
        {
            StartCoroutine(Decelerate());
        }

        if(Input.GetKey(KeyCode.T))
        {
            flocking = true;
            following = false;
        }

        if (Input.GetKey(KeyCode.Y))
        {
            following = true;
            flocking = false; 
        }

        if (flocking)
        {
            flock.Enter();
            pathFollow.Exit();
        }

        if (following)
        {
            pathFollow.Enter();
            flock.Exit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IPredator reactToPredator = other.GetComponent<IPredator>();
        if (reactToPredator != null)
        {
            Debug.Log("I got eaten!!!");
            //Destroy the fish
        }
        
    }
    
    IEnumerator Decelerate()
    {
        fRb.AddRelativeForce(new Vector3(0f,0f,-1f));
        yield return new WaitForSeconds(reduction);
    }
    
    
    //IReactToWater Interface
    public bool IsWet { get; set; }
    
    //IControllable Interface
    public void Steer(float input)
    {
        float currentYEuler = transform.eulerAngles.y;
        float targetAngle = 0;
        
        targetAngle = -input * steeringMax;
        

        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, 0.1f);
        tailObjectTransform.eulerAngles = new Vector3(0, currentYEuler + 2f * currentSteeringAngle, 0);
    }
    public void Accelerate(float input)
    {
        fRb.AddForceAtPosition(input*acceleration*transform.TransformDirection(Vector3.forward), transform.position,0);
    }

    public void Reverse(float input)
    {
        fRb.AddForceAtPosition(input*acceleration*transform.TransformDirection(Vector3.back), transform.position,0);
    }
    
    public void Action(InputActionPhase aActionPhase)
    {
        throw new System.NotImplementedException();
    }

    public void Action2(InputActionPhase aActionPhase)
    {
        throw new System.NotImplementedException();
    }

    public void Action3(InputActionPhase aActionPhase)
    {
        throw new System.NotImplementedException();
    }
    
    //StateBase Interface
    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Execute()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

//RTS Interface
    public void Selected()
    {
        throw new System.NotImplementedException();
    }

    public void Deselected()
    {
        throw new System.NotImplementedException();
    }

    public void SetDestination(Vector3 position)
    {
        throw new System.NotImplementedException();
    }



    #region IEdible
    
    public void GetEaten(IPredator eatenBy)
    {
        throw new System.NotImplementedException();
    }

    public EdibleInfo GetInfo()
    {
        throw new System.NotImplementedException();
    }
    public void GotShatOut(IPredator shatOutBy)
    {
        throw new System.NotImplementedException();
    }

    #endregion
  
}
