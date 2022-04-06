using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeShark : MonoBehaviour
{
	public Rigidbody rb;
	public Transform tailTransform;
	
	public float acceleratingForce = 5f;
	public float longitudinalFrictionCoefficient = 0.4f;
	public float lateralFrictionCoefficient = 15f;
	public float steeringFrictionCoefficient = 3f;
	public float wigglePeriodInSeconds = 2f;
	public float maxWiggleAngle = 5f;
	public float maxSteeringAngle = 45f;

	public float currentSteeringAngle = 0f;
	public Vector3 localVelocity;
	public Vector3 tailLocalVelocity;
	

	private void SwimForward(float amount)
	{
		rb.AddForceAtPosition(amount*acceleratingForce*transform.TransformDirection(Vector3.forward), transform.position, 0);
	}

	private void Steer(float amount)
	{
		float targetAngle = -amount * maxSteeringAngle +
		                    maxWiggleAngle * Mathf.Sin(Time.time * 2 * Mathf.PI / wigglePeriodInSeconds);
		currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetAngle,0.1f);
		tailTransform.eulerAngles = new Vector3(0, transform.eulerAngles.y+currentSteeringAngle, 0);
	}
	
	// Start is called before the first frame update
    void Start()
    {
	    rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	    localVelocity = transform.InverseTransformDirection(rb.velocity);
	    tailLocalVelocity = tailTransform.InverseTransformDirection(rb.velocity);
	    
	    //drive force.
	    rb.AddForceAtPosition(longitudinalFrictionCoefficient*rb.mass*transform.
		    TransformDirection(new Vector3 (0, 0, -localVelocity.z)), tailTransform.position);
	    
	    //body lateral friction
	    rb.AddRelativeForce(lateralFrictionCoefficient*rb.mass*new Vector3 (-localVelocity.x, 0, 0));

	    //tail steering friction.
	    rb.AddForceAtPosition(steeringFrictionCoefficient*rb.mass*tailTransform.
		    TransformDirection(new Vector3 (-tailLocalVelocity.x, 0, 0)), tailTransform.position);

	    if (Input.GetKey(KeyCode.UpArrow)||Input.GetKey(KeyCode.W))
	    {
		    SwimForward(1);
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
}
