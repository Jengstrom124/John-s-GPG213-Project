using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LukeShark : NetworkBehaviour, IControllable, IPredator, IEdible
{
	public AudioSource audio;
	public AudioClip boost;
	public AudioClip oneEighty;
	
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
	public bool isBoosting = false;
	public float boostFactor = 3f;
	public float boostTimeSeconds = 1f;
	public float boostCooldownSeconds = 2f;
	
	public bool oneEightyReady = true;
	public bool isOneEightying = false;
	public float oneEightyFactor = 3f;
	public float oneEightyVelocityFactor = 5f;
	public float oneEightyTimeSeconds = 0.7f;
	public float oneEightyCooldownSeconds = 3f;

	private Vector3 accelForce;
	private Vector3 reverseForce;
	private float steerTarget;

	public void Accelerate(float input)
	{
		if (IsServer)
		{
			if (isBoosting || isOneEightying)
			{
				//accelForce = acceleratingForce * transform.TransformDirection(Vector3.forward);
			}
			else
			{
				accelForce = input * acceleratingForce * transform.TransformDirection(Vector3.forward);
			}
		}
	}

	public void Reverse(float input)
	{
		if (IsServer)
		{
			if (!(isBoosting | isOneEightying))
			{
				reverseForce = -input * reversingForce * transform.TransformDirection(Vector3.forward);
			}
		}
	}
	
	public void Steer(float input)
	{
		float currentYEuler = transform.eulerAngles.y;
		if (!isOneEightying)
		{
			if (Mathf.Abs(input) < 0.5f)
			{
				steerTarget = -input * maxSteeringAngle +
				              maxWiggleAngle * wiggleDirection *
				              Mathf.Sin((Time.time - timeReset) * 2 * Mathf.PI / wigglePeriodInSeconds);
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
		}

		preJointTransform.eulerAngles = new Vector3(90, currentYEuler + 0.5f * currentSteeringAngle, 180);
		mainJointTransform.eulerAngles = new Vector3(0, currentYEuler + currentSteeringAngle, 0);
		postJointTransform.eulerAngles = new Vector3(-90, currentYEuler + 1.5f * currentSteeringAngle, 0);
		tailTipTransform.eulerAngles = new Vector3(-90, currentYEuler + 2f * currentSteeringAngle, 0);
		headJointTransform.eulerAngles = new Vector3(90, 0, -(currentYEuler - 0.5f * currentSteeringAngle));
		}

	#region Boost Ability

	private IEnumerator Boost()
	{
		if (IsServer)
		{
			acceleratingForce *= boostFactor;
			isBoosting = true;
			accelForce = acceleratingForce * transform.TransformDirection(Vector3.forward);
			//PopFishFromGuts
		}

		if (IsClient)
		{
			audio.PlayOneShot(boost);
			audio.Play();
		}

		yield return new WaitForSeconds(boostTimeSeconds);
		
		if (IsServer)
		{
			isBoosting = false;
			acceleratingForce /= boostFactor;
		}

		if (IsClient)
		{
			audio.Stop();
		}

		StartCoroutine(BoostCooldown());
	}
	
	private IEnumerator BoostCooldown()
	{
		yield return new WaitForSeconds(boostCooldownSeconds);

		boostReady = true;
	}
	
	#endregion
	
	#region 180 Ability
	
	private IEnumerator OneEighty()
	{
		float temp = lateralFrictionCoefficient;
		if (IsServer)
		{
			isOneEightying = true;
			accelForce = acceleratingForce * transform.TransformDirection(Vector3.forward);
			float turnStrength = oneEightyFactor*(acceleratingForce-Vector3.Magnitude(rb.velocity)/oneEightyVelocityFactor)/acceleratingForce;
			if (steerTarget < 0)
			{
				steerTarget = -maxSteeringAngle*turnStrength;
			}
			else
			{
				steerTarget = maxSteeringAngle*turnStrength;
			}
			lateralFrictionCoefficient *= turnStrength;
			//PopFishFromGuts
		}

		if (IsClient)
		{
			audio.PlayOneShot(oneEighty);
		}

		yield return new WaitForSeconds(oneEightyTimeSeconds);
		
		if (IsServer)
		{
			lateralFrictionCoefficient = temp;
			isOneEightying = false;
		}

		StartCoroutine(OneEightyCooldown());
	}
	
	private IEnumerator OneEightyCooldown()
	{
		yield return new WaitForSeconds(oneEightyCooldownSeconds);

		oneEightyReady = true;
	}
	
	#endregion

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
		if (oneEightyReady && reverseForce.magnitude < reversingForce*0.05f)
		{
			oneEightyReady = false;
			StartCoroutine(OneEighty());
		}
	}

	public void Action3()
	{
		audio.PlayOneShot(oneEighty);
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
		    Vector3 position = transform.position;
		    Vector3 tailPosition = mainJointTransform.position;
		    
		    rb.AddForceAtPosition(accelForce, position);
		    rb.AddForceAtPosition(reverseForce, position);
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
			    mainJointTransform.TransformDirection(new Vector3(-tailLocalVelocity.x, 0, 0)), tailTipTransform.position);
	    }
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
