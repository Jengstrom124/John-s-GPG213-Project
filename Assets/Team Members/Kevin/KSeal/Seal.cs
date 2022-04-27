using System.Collections;
using System.Collections.Generic;
using Anthill.Effects;
using Unity.Netcode;
using UnityEngine;

namespace Kevin
{
    public class Seal : NetworkBehaviour, IControllable, IReactsToWater
{
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
    //public bool isWet;
    
    //Audio
    public AudioSource jumpSound;

    void Awake()
    {
        sealPrefab.transform.position = new Vector3(75f,15f,105f);
    }
    void Start()
    {
        if (IsServer)
        {
            jumpSound = GetComponent<AudioSource>();
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
        
            //localVelocity = transform.InverseTransformDirection(sealRigidbody.velocity);
        
            //sealRigidbody.AddRelativeForce(new Vector3(-localVelocity.x,0f,0f));
        
            sealRigidbody.AddForceAtPosition(accelerationForce, transform.position);
            sealRigidbody.AddForceAtPosition(reverseForce, transform.position);
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle, lerpValue);
            
            if (localVelocity.z > 0) 
            {
                StartCoroutine(Decelerate());
            }
        }
        
        if (IsWet == false)
        {
            capsuleCollider.material = new PhysicMaterial("none");
        }
        else
        {
            capsuleCollider.material = new PhysicMaterial("SlipperyMaterial");
        }
        /*if (Input.GetKeyDown(KeyCode.Space) && isJumping == false)
        {
            Jump();
            StartCoroutine(JumpTimer());
        }*/

        

        /*if (IsWet && isJumping == false)
        {
            InWater();
        }*/

        /*if (isJumping == true || isWet == false)
        {
            MoveCollider();
            StartCoroutine(SmoothEntry());
        }
      
        GetComponent<CapsuleCollider>().center = colliderCenter;

        if (IsWet == true)
        {
            isWet = true;
        }
        else
        {
            isWet = false;
        }*/
    }

    /*public void MoveCollider()
    {
        colliderCenter = new Vector3(0f, 0f, -1f);
        GetComponent<CapsuleCollider>().center = colliderCenter;
    }
    IEnumerator SmoothEntry()
    {
        yield return new WaitForSeconds(0.75f);
        colliderCenter = new Vector3(0f, 0.6f, -1f);
        GetComponent<CapsuleCollider>().center = colliderCenter;
    }*/

    /*public void InWater()
    {
        accelerationSpeed = 5f;
        currentSteeringMax = steeringMax;
        sealRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }*/
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

    IEnumerator Decelerate()
    {
        if (IsServer)
        {
            sealRigidbody.AddRelativeForce(new Vector3(0f, 0f, -1f));
            yield return new WaitForSeconds(reduction);
        }
    }

    IEnumerator BoosterLimit()
    {
        if (IsServer)
        {
            yield return new WaitForSeconds(3f);
            accelerationSpeed = 5f;
        }
    }

    public void Steer(float input)
    {
        if (IsServer)
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

    public void Accelerate(float input)
    {
        if (IsServer)
        {
            
            accelerationForce = input * accelerationSpeed * transform.TransformDirection(Vector3.forward);
            /*if (IsWet)
            {
                sealRigidbody.AddForceAtPosition(input*accelerationSpeed*transform.TransformDirection(Vector3.forward), transform.position,0);
            }
            else
            {
                sealRigidbody.AddForceAtPosition(input*accelerationSpeed*2f*transform.TransformDirection(Vector3.forward), transform.position,0);
            }*/
        }
    }

    public void Reverse(float input)
    {
        if (IsServer)
        {
            reverseForce = -input * accelerationSpeed * transform.TransformDirection(Vector3.back);
        }
    }

    public void Action()
    {
        accelerationForce =  20f * accelerationSpeed * transform.TransformDirection(Vector3.forward);
        StartCoroutine(BoosterLimit());
    }

    public void Action2()
    {
        if(IsServer && isJumping == false)
        {
            jumpSound.Play();
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

    public bool IsWet { get; set; }
}
}

