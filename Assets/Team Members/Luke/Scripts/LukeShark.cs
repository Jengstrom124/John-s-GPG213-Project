using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class LukeShark : NetworkBehaviour, IControllable, IPredator, IEdible
{
	public AudioSource audioSource;
	public AudioClip boost;
	public AudioClip oneEighty;
	
	public Rigidbody rb;
	private FishContainer stomach;
	public Transform preJointTransform;
	public Transform mainJointTransform;
	public Transform postJointTransform;
	public Transform tailTipTransform;
	public Transform headJointTransform;

	public Transform jaw;
	public Transform finBase;
	
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

	private float currentSteeringAngle = 0f;
	private Vector3 localVelocity;
	private Vector3 tailLocalVelocity;

	public bool boostReady = true;
	public bool isBoosting = false;
	public float boostFactor = 3f;
	public float boostTimeSeconds = 1f;
	public float boostCooldownSeconds = 2f;
	public int boostFoodCost = 1;
	
	public bool oneEightyReady = true;
	public bool isDoingASickOneEighty = false;
	public float oneEightyFactor = 3f;
	public float oneEightyVelocityFactor = 5f;
	public float oneEightyCooldownSeconds = 3f;
	public int oneEightyFoodCost = 1;

	private Vector3 accelForce;
	private Vector3 reverseForce;
	private float steerTarget;

	private float jawAmp = 1f;
	private float normalJawAmp = 1f;
	public float eatingJawAmp = 5f;
	private float jawFreq = 0.5f/Mathf.PI;
	
	//hunger
	public int foodLevel = 10;

	public void Accelerate(float input)
	{
		if (IsServer)
		{
			if (isBoosting || isDoingASickOneEighty)
			{
				accelForce = acceleratingForce * transform.TransformDirection(Vector3.forward);
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
			if (!(isBoosting | isDoingASickOneEighty))
			{
				reverseForce = -input * reversingForce * transform.TransformDirection(Vector3.forward);
			}
		}
	}
	
	public void Steer(float input)
	{
		float currentYEuler = transform.eulerAngles.y;
		if (!isDoingASickOneEighty)
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
			stomach.PopFishFromGuts(boostFoodCost);
		}
		if (IsClient)
		{
			audioSource.PlayOneShot(boost);
			audioSource.Play();
		}
		yield return new WaitForSeconds(boostTimeSeconds);
		if (IsServer)
		{
			isBoosting = false;
			acceleratingForce /= boostFactor;
		}
		if (IsClient)
		{
			audioSource.Stop();
		}
		StartCoroutine(BoostCooldown());
	}
	
	private IEnumerator BoostCooldown()
	{
		yield return new WaitForSeconds(boostCooldownSeconds);

		boostReady = true;
	}
	
	#endregion
	
	#region OneEighty Ability
	
	private IEnumerator OneEighty()
	{
		float temp = lateralFrictionCoefficient;
		float tempAngle = transform.rotation.eulerAngles.y+360;
		if (IsServer)
		{
			isDoingASickOneEighty = true;
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
			stomach.PopFishFromGuts(oneEightyFoodCost);
		}

		if (IsClient)
		{
			audioSource.PlayOneShot(oneEighty);
		}

		int t = 0;
		
		while (!((tempAngle-transform.rotation.eulerAngles.y)%360>60 && (tempAngle-transform.rotation.eulerAngles.y)%360<300 || t>60))
		{
			t++;
			yield return null;
		}
		

		if (IsServer)
		{
			lateralFrictionCoefficient = temp;
			isDoingASickOneEighty = false;
		}

		StartCoroutine(OneEightyCooldown());
	}
	
	private IEnumerator OneEightyCooldown()
	{
		yield return new WaitForSeconds(oneEightyCooldownSeconds);

		oneEightyReady = true;
	}
	
	#endregion


	public void ChangeSize()
	{
		float scaleValue = 1f + Mathf.Sqrt(Mathf.Clamp(foodLevel, 0, 300)) / 10f;
		transform.localScale = Vector3.one * scaleValue;
	}

	public void FauxBuoyancy()
	{
		
	}
	
	public void Action(InputActionPhase aActionPhase)
	{
		//Boost
		if (boostReady && foodLevel > 0)
		{
			boostReady = false;
			StartCoroutine(Boost());
		}
	}

	public void Action2(InputActionPhase aActionPhase)
	{
		if (oneEightyReady && reverseForce.magnitude < reversingForce*0.05f)
		{
			oneEightyReady = false;
			StartCoroutine(OneEighty());
		}
	}

	public void Action3(InputActionPhase aActionPhase)
	{
		audioSource.PlayOneShot(oneEighty);
	}

	// Start is called before the first frame update
    void Start()
    {
	    rb = GetComponent<Rigidbody>();
	    audioSource = GetComponent<AudioSource>();
	    stomach = GetComponent<FishContainer>();
	    StartCoroutine(DepleteFood());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, steerTarget, lerpValue);
	    jaw.eulerAngles = new Vector3(jawAmp*Mathf.Sin(Time.time*jawFreq), -90, -90);
	    
	    if (IsServer)
	    {
		    Vector3 position = transform.position;
		    Vector3 tailPosition = mainJointTransform.position;
		    
		    rb.AddForceAtPosition(accelForce, position);
		    rb.AddForceAtPosition(reverseForce, position);

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

    private void OnTriggerEnter(Collider other)
    {
	    if (!other.isTrigger)
	    {
		    //should check for size difference here
		    StartCoroutine(Chomp());

		    IEdible food = other.gameObject.GetComponent<IEdible>();
		    if (food != null)
		    {
			    food.GetEaten(this);
			    foodLevel += food.GetInfo().amount;
			    ChangeSize();
		    }

		    FishBase fishFood = other.gameObject.GetComponent<FishBase>();
		    if (fishFood != null)
		    {
			    stomach.AddToStomach(fishFood);
		    }
	    }
    }

    private IEnumerator Chomp()
    {
	    jawAmp = eatingJawAmp;
	    yield return new WaitForSeconds(jawFreq*3);
	    jawAmp = normalJawAmp;
    }

    private IEnumerator DepleteFood()
    {
	    yield return new WaitForSeconds(1f);
	    if (foodLevel > 0)
	    {
		    foodLevel--;
		    ChangeSize();
	    }
	    StartCoroutine(DepleteFood());
    }

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

	public Vector3 GetBumPosition()
	{
		return postJointTransform.position;
	}
}
