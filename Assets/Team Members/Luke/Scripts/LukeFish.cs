using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class LukeFish : FishBase, IControllable, IRTS, IEdible
{
	public GameObject flockObject;
	public GameObject pathfollowObject;
	public IStateBase flock;
	public IStateBase pathfollow;
	
	public PathTracker pathTracker;
	
	public Rigidbody rb;
	public Transform mainJointTransform;
	public Transform tailTipTransform;
	
	public float acceleratingForce = 5f;
	public float reversingForce = 3f;
	public float longitudinalFrictionCoefficient = 0.4f;
	public float lateralFrictionCoefficient = 15f;
	public float steeringFrictionCoefficient = 3f;
	public float wigglePeriodInSeconds = 2f;
	public float maxWiggleAngle = 5f;
	public float maxSteeringAngle = 45f;

	public float currentSteeringAngle;
	public Vector3 localVelocity;
	public Vector3 tailLocalVelocity;

	public Vector3 accelForce;
	private Vector3 reverseForce;
	private float steerTarget;

	public int foodValue = 10;

	public void Accelerate(float input)
	{
		if (IsServer)
		{
			accelForce = input * acceleratingForce * transform.TransformDirection(Vector3.forward);
		}
	}

	public void Reverse(float input)
	{
		if (IsServer)
		{
			reverseForce = -input * reversingForce * transform.TransformDirection(Vector3.forward);
		}
	}
	
	public void Steer(float input)
	{
		float currentYEuler = transform.eulerAngles.y;
		
		if (Mathf.Abs(input) < 0.5)
		{
			steerTarget = -input * maxSteeringAngle +
			              maxWiggleAngle * Mathf.Sin(Time.time * 2 * Mathf.PI / wigglePeriodInSeconds);
		}
		else
		{
			steerTarget = -input * maxSteeringAngle;
		}

		mainJointTransform.eulerAngles = new Vector3(0, currentYEuler+currentSteeringAngle, 0);
	}
	

	public void Action(InputActionPhase aActionPhase)
	{
		
	}

	public void Action2(InputActionPhase aActionPhase)
	{
	}

	public void Action3(InputActionPhase aActionPhase)
	{
	}

	void OnEnable()
	{
		//pathTracker.destinationReachedEvent += ;
	}
	
	void OnDisable()
	{
		//pathTracker.destinationReachedEvent -= ;
	}

	// Start is called before the first frame update
    void Start()
    {
	    rb = GetComponent<Rigidbody>();
	    flock = flockObject.GetComponent<IStateBase>();
	    pathfollow = pathfollowObject.GetComponent<IStateBase>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    if (IsServer)
	    {
		    Vector3 position = transform.position;
		    Vector3 tailPosition = mainJointTransform.position;

		    localVelocity = transform.InverseTransformDirection(rb.velocity);
		    tailLocalVelocity = mainJointTransform.InverseTransformDirection(rb.GetPointVelocity(tailPosition));

		    rb.AddForceAtPosition(accelForce, position);
		    rb.AddForceAtPosition(reverseForce, position);
		    currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, steerTarget, 0.1f);

		    //drive friction.
		    rb.AddForceAtPosition(
			    longitudinalFrictionCoefficient * rb.mass *
			    transform.TransformDirection(new Vector3(0, 0, -localVelocity.z)), tailPosition);

		    //body lateral friction
		    rb.AddRelativeForce(lateralFrictionCoefficient * rb.mass * new Vector3(-localVelocity.x, 0, 0));

		    //tail steering friction.
		    rb.AddForceAtPosition(
			    steeringFrictionCoefficient * rb.mass *
			    mainJointTransform.TransformDirection(new Vector3(-tailLocalVelocity.x, 0, 0)),
			    tailTipTransform.position);
	    }
    }

    public void GetEaten(IPredator eatenBy)
    {
	    
    }

	public EdibleInfo GetInfo()
	{
		return new EdibleInfo {edibleType = EdibleType.Food, amount = foodValue};
	}

	public void GotShatOut(IPredator shatOutBy)
	{
		
	}

	public void Selected()
	{
		flock.Exit();
		pathfollow.Enter();
	}

	public void Deselected()
	{
		pathfollow.Exit();
		flock.Enter();
	}

	public void SetDestination(Vector3 position)
	{
		pathTracker.GetPathToDestination(position);
	}
}
