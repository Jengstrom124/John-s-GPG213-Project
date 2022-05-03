using System.Collections;
using System.Collections.Generic;
using Anthill.Effects;
using Unity.Netcode;
using UnityEngine;

namespace Kevin
{
    public class Seal : NetworkBehaviour, IControllable, IReactsToWater
{
    #region Variables

    //Gameobject Variables
    public Rigidbody sealRigidbody;
    
    public GameObject sealPrefab;
    
    //Vectors
    public Vector3 localVelocity;
    public Vector3 tailLocalVelocity;
    public Vector3 tailPosition;
    public Vector3 accelerationForce;
    public Vector3 reverseForce;

    //Transforms
    public Transform headTransform; 
    public Transform tailTransform;
    public Transform jumpTransform;
    
    //Wiggle Transforms
    public Transform tailLeadTransform;
    public Transform tailMidTransform;
    public Transform tailTipTransform;
    
    //Stats/Floats Variables
    public float accelerationSpeed;
    public float lerpValue = 0.1f;
    public float currentSteeringAngle;
    public float targetAngle;
    public float steeringMax = 30f;
    public float currentSteeringMax;
    public float airSteeringMax = 15f;
    public float reduction = 0.5f;

    public float currentBobbleAngle;
    
    //Check if jumping
    public bool isJumping;
    
    //Collider Config
    public CapsuleCollider capsuleCollider;
    public Vector3 colliderCenter;

    public Transform spawnPoint;
    
    
    //Audio
    //public AudioSource jumpSound;

    #endregion
    /*void Awake()
    {
        sealPrefab.transform.position = new Vector3(75f,15f,105f);
    }*/
    void Start()
    {
        if (IsServer)
        {
            colliderCenter = GetComponent<CapsuleCollider>().center;
            sealRigidbody = sealPrefab.GetComponent<Rigidbody>();
            tailPosition = sealPrefab.transform.position;
        }

    }
    void FixedUpdate()
    {
        if (IsServer)
        {
            Vector3 tailPosition = tailTransform.position;
        
            localVelocity = transform.InverseTransformDirection(sealRigidbody.velocity);
            tailLocalVelocity = tailTransform.InverseTransformDirection(sealRigidbody.GetPointVelocity(tailPosition));
        
            sealRigidbody.AddRelativeForce(sealRigidbody.mass*new Vector3 (-localVelocity.x, 0, 0));
            sealRigidbody.AddForceAtPosition(sealRigidbody.mass*tailTransform.TransformDirection(new Vector3 (-tailLocalVelocity.x, 0, 0)), tailPosition);
            
            sealRigidbody.AddForceAtPosition(accelerationForce, transform.position);
            sealRigidbody.AddForceAtPosition(reverseForce, transform.position);
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, lerpValue);
            
            if (localVelocity.z > 0) 
            {
                StartCoroutine(Decelerate());
            }

            if (IsWet == false && isJumping == false)
            {
                sealRigidbody.drag = 2f;
                sealRigidbody.angularDrag = 2f;
            }
            else
            {
                sealRigidbody.drag = 0f;
                sealRigidbody.angularDrag = 0.5f;
            }
        }
    }
    IEnumerator Decelerate()
    {
        if (IsServer)
        {
            sealRigidbody.AddRelativeForce(new Vector3(0f, 0f, -1f));
            yield return new WaitForSeconds(reduction);
        }
    }

    /*IEnumerator BoosterLimit()
    {
        if (IsServer)
        {
            yield return new WaitForSeconds(3f);
            accelerationSpeed = 10f;
        }
    }*/

    #region JumpFunctions

    public void Jump()
    {
        if (IsServer)
        {
            isJumping = true;
            sealRigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationX |
                                        RigidbodyConstraints.FreezeRotationZ;
            accelerationSpeed = 1f;
            currentSteeringMax = airSteeringMax;
            sealRigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Sqrt(20f * -2 * -9.81f) * 20f, 0f),
                jumpTransform.position);
            StartCoroutine(GravityDrop());
        }
    }

    IEnumerator GravityDrop()
    {
        if (IsServer)
        {
            yield return new WaitForSeconds(0.5f);
            sealRigidbody.AddForceAtPosition(350f * transform.TransformDirection(Vector3.down), jumpTransform.position,
                0f);
            accelerationSpeed = 10f;
        }
    }

    IEnumerator JumpTimer()
    {
        if (IsServer)
        {
            yield return new WaitForSeconds(0.75f);
            isJumping = false;
        }
    }

    #endregion

    #region IControllable Interface

    public void Steer(float input)
    {
        if (IsServer)
        {
            if (IsWet == false && input < 0 && isJumping == false)
            {
                sealRigidbody.AddRelativeTorque(new Vector3(0f,-3f,0f));
                float currentYEuler = transform.eulerAngles.y;
            
                targetAngle = -input * currentSteeringMax;
            
           
                tailTransform.eulerAngles = new Vector3(0, currentYEuler + 2f * currentSteeringAngle, 0);
                tailLeadTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.25f * currentSteeringAngle ,0f);
                tailMidTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.5f * currentSteeringAngle ,0f);
                tailTipTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.75f * currentSteeringAngle ,0f);
                headTransform.eulerAngles = new Vector3(60f,currentYEuler, currentSteeringAngle/2f);
            }
            else if (IsWet == false && input > 0 && isJumping == false)
            {
                sealRigidbody.AddRelativeTorque(new Vector3(0f,3f,0f));
                float currentYEuler = transform.eulerAngles.y;
            
                targetAngle = -input * currentSteeringMax;
            
           
                tailTransform.eulerAngles = new Vector3(0, currentYEuler + 2f * currentSteeringAngle, 0);
                tailLeadTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.25f * currentSteeringAngle ,0f);
                tailMidTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.5f * currentSteeringAngle ,0f);
                tailTipTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.75f * currentSteeringAngle ,0f);
                headTransform.eulerAngles = new Vector3(60f,currentYEuler, currentSteeringAngle/2f);
            }
            else
            {
                float currentYEuler = transform.eulerAngles.y;
            
                targetAngle = -input * currentSteeringMax;
            
           
                tailTransform.eulerAngles = new Vector3(0, currentYEuler + 2f * currentSteeringAngle, 0);
                tailLeadTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.25f * currentSteeringAngle ,0f);
                tailMidTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.5f * currentSteeringAngle ,0f);
                tailTipTransform.eulerAngles = new Vector3(90f,currentYEuler + 0.75f * currentSteeringAngle ,0f);
                headTransform.eulerAngles = new Vector3(60f,currentYEuler, currentSteeringAngle/2f);
            }
          
            
        }
    }
    public void Accelerate(float input)
    {
        if (IsServer)
        {
            if (IsWet)
            {
                accelerationForce = input * accelerationSpeed * transform.TransformDirection(Vector3.forward);
            }

            if (IsWet == false && isJumping == false)
            {
                accelerationForce = 2f * input * accelerationSpeed * transform.TransformDirection(Vector3.forward);
            }
        }
    }

    public void Reverse(float input)
    {
        if (IsServer)
        {
            if (IsWet)
            {
                accelerationForce = -input * accelerationSpeed * transform.TransformDirection(Vector3.forward);
            }

            if (IsWet == false && isJumping == false)
            {
                accelerationForce = -input * accelerationSpeed * transform.TransformDirection(Vector3.forward);
            }
        }
    }
    
    public void Action()
    {
        /*accelerationForce =  20f * accelerationSpeed * transform.TransformDirection(Vector3.forward);
        StartCoroutine(BoosterLimit());*/
    }

    public void Action2()
    {
        if(IsServer && isJumping == false)
        {
            //jumpSound.Play();
            Jump();
            StartCoroutine(JumpTimer());
        }
        
        //View
        if (IsClient)
        {
            
        }
        
        //Model
        if (IsServer)
        {
            
        }
    }

    public void Action3()
    {
       
    }
    #endregion

    #region IReactToWater Interface

    public bool IsWet { get; set; }

    #endregion
    
}
}

