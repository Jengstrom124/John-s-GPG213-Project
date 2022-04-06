using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeShark : MonoBehaviour, IControllable
{
	public Rigidbody rb;
	public Transform tailJointTransform;
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

	public float currentSteeringAngle = 0f;
	public Vector3 localVelocity;
	public Vector3 tailLocalVelocity;
	

	public void Accelerate(float input)
	{
		rb.AddForceAtPosition(input*acceleratingForce*transform.TransformDirection(Vector3.forward), transform.position, 0);
	}

	public void Reverse(float input)
	{
		rb.AddForceAtPosition(-input*reversingForce*transform.TransformDirection(Vector3.forward), transform.position, 0);
	}
	
	private void Steer(float input)
	{
		float currentYEulers = transform.eulerAngles.y;
		
		float targetAngle = -input * maxSteeringAngle +
		                    maxWiggleAngle * Mathf.Sin(Time.time * 2 * Mathf.PI / wigglePeriodInSeconds);
		currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle,0.1f);
		
		tailJointTransform.eulerAngles = new Vector3(0, currentYEulers+currentSteeringAngle, 0);
		headJointTransform.eulerAngles = new Vector3(90, 0, -(currentYEulers-0.5f*currentSteeringAngle));
	}

	public void Action()
	{
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
	    Vector3 tailPosition = tailJointTransform.position;
	    Vector3 tailTipPosition = tailTipTransform.position;
	    localVelocity = transform.InverseTransformDirection(rb.velocity);
	    tailLocalVelocity = tailJointTransform.InverseTransformDirection(rb.GetPointVelocity(tailPosition));
	    
	    //drive friction.
	    rb.AddForceAtPosition(longitudinalFrictionCoefficient*rb.mass*transform.
		    TransformDirection(new Vector3 (0, 0, -localVelocity.z)), tailPosition);
	    
	    //body lateral friction
	    rb.AddRelativeForce(lateralFrictionCoefficient*rb.mass*new Vector3 (-localVelocity.x, 0, 0));

	    //tail steering friction.
	    rb.AddForceAtPosition(steeringFrictionCoefficient*rb.mass*tailJointTransform.
		    TransformDirection(new Vector3 (-tailLocalVelocity.x, 0, 0)), tailTipPosition);

	    if (Input.GetKey(KeyCode.UpArrow)||Input.GetKey(KeyCode.W))
	    {
		    Accelerate(1);
	    }
	    else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
	    {
		    Reverse(1);
	    }
	    
		if (Input.GetKey(KeyCode.LeftArrow)||Input.GetKey(KeyCode.A))
	    {
		    Steer(-1);
	    }
	    else if (Input.GetKey(KeyCode.DownArrow)||Input.GetKey(KeyCode.D))
	    {
		    Steer(1);
	    }
	    else
	    {
		    Steer(0);
	    }
    }

    void IControllable.Steer(float input)
    {
	    Steer(input);
    }
}
