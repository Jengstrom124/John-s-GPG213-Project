using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LukeShark : NetworkBehaviour, IControllable, IPredator, IEdible
{
	public Rigidbody rb;
	public Transform preJointTransform;
	public Transform mainJointTransform;
	public Transform postJointTransform;
	public Transform tailTipTransform;
	public Transform headJointTransform;
	
	public float acceleratingForce = 5f;
	public float reversingForce = 3f;
	public float longitudinalFrictionCoefficient = 0.4f;
	public float lateralFrictionCoefficient = 15f;
	public float steeringFrictionCoefficient = 3f;
	public float wigglePeriodInSeconds = 2f;
	public float maxWiggleAngle = 5f;
	public float maxSteeringAngle = 45f;
	private float timeReset;
	private int wiggleDirection = 1; //1 or -1

	private float lerpValue = 0.1f;

	public float currentSteeringAngle = 0f;
	public Vector3 localVelocity;
	public Vector3 tailLocalVelocity;

	public bool boostReady = true;
	public float boostFactor = 3f;
	public float boostTimeSeconds = 1f;
	public float boostCooldownSeconds = 2f;

	private Vector3 accelForce;
	private Vector3 reverseForce;
	private float steerTarget;
	
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
			reverseForce = -input * reversingForce * Time.deltaTime * transform.TransformDirection(Vector3.forward);
		}
	}
	
	private void Steer(float input)
	{
		float currentYEuler = transform.eulerAngles.y;
		if (Mathf.Abs(input) < 0.5f)
		{
			steerTarget = -input * maxSteeringAngle +
			                    maxWiggleAngle * wiggleDirection * Mathf.Sin((Time.time-timeReset) * 2 * Mathf.PI / wigglePeriodInSeconds);
		}
		else
		{
			steerTarget = -input * maxSteeringAngle;
			timeReset = Time.time;
			if (input < 0)
			{
				wiggleDirection = -1;
			}
			else
			{
				wiggleDirection = 1;
			}
		}
		
		preJointTransform.eulerAngles = new Vector3(90, currentYEuler + 0.5f * currentSteeringAngle, 180);
		mainJointTransform.eulerAngles = new Vector3(0, currentYEuler + currentSteeringAngle, 0);
		postJointTransform.eulerAngles = new Vector3(-90, currentYEuler + 1.5f * currentSteeringAngle, 0);
		tailTipTransform.eulerAngles = new Vector3(-90, currentYEuler + 2f * currentSteeringAngle, 0);
		headJointTransform.eulerAngles = new Vector3(90, 0, -(currentYEuler - 0.5f * currentSteeringAngle));
	}


	private IEnumerator Boost()
	{
		if (IsServer)
		{
			acceleratingForce *= boostFactor;
			//PopFishFromGuts
		}

		yield return new WaitForSeconds(boostTimeSeconds);
		if (IsServer)
		{
			acceleratingForce /= boostFactor;
		}

		StartCoroutine(BoostCooldown());
	}
	
	private IEnumerator BoostCooldown()
	{
		yield return new WaitForSeconds(boostCooldownSeconds);

		boostReady = true;
	}
	
	public void Action()
	{
		//Boost
		if (boostReady)
		{
			boostReady = false;
			StartCoroutine(Boost());
		}
	}

	public void Action2()
	{
	}

	public void Action3()
	{
	}

	// Start is called before the first frame update
    void Start()
    {
	    rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    if (IsServer)
	    {
		    Vector3 tailPosition = mainJointTransform.position;
		    Vector3 tailTipPosition = tailTipTransform.position;
		    
		    rb.AddForceAtPosition(accelForce, transform.position);
		    rb.AddForceAtPosition(reverseForce, transform.position);
		    currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, steerTarget, lerpValue);
		    
		    localVelocity = transform.InverseTransformDirection(rb.velocity);
		    tailLocalVelocity = mainJointTransform.InverseTransformDirection(rb.GetPointVelocity(tailPosition));

		    //drive friction.
		    rb.AddForceAtPosition(
			    longitudinalFrictionCoefficient * rb.mass *
			    transform.TransformDirection(new Vector3(0, 0, -localVelocity.z)), tailPosition);

		    //body lateral friction
		    rb.AddRelativeForce(lateralFrictionCoefficient * rb.mass * new Vector3(-localVelocity.x, 0, 0));

		    //tail steering friction.
		    rb.AddForceAtPosition(
			    steeringFrictionCoefficient * rb.mass *
			    mainJointTransform.TransformDirection(new Vector3(-tailLocalVelocity.x, 0, 0)), tailTipPosition);
	    }
    }

    void IControllable.Steer(float input)
    {
	    Steer(input);
    }

    public void GotFood(float amount)
    {
	    
    }

    public void ChangeBoost(float amount)
    {
	    
    }

    public void GetEaten(IPredator eatenBy)
    {
	    
    }

	public EdibleInfo GetInfo()
	{
		return new EdibleInfo();
	}

	public Vector3 GetBumPosition()
	{
		return postJointTransform.position;
	}
}
