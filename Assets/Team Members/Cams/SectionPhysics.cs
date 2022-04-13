using UnityEngine;
using System.Collections;

public class SectionPhysics : MonoBehaviour
{
	public Rigidbody mainBody;
	public float suspensionLength;
	public float suspensionStrength;
	public float damping = 0;
	public bool inAir;

	public float lateralFriction;
	private Transform myTransform;

	public Vector3 localVelocity;

	private void Start()
	{
		myTransform = GetComponent<Transform>();
	}

	RaycastHit raycastHit;

	public bool useSuspension = false;

	// Update is called once per frame
	void Update()
	{
		//		transform.Rotate(new Vector3(0,10,0), Space.Self);

		localVelocity = myTransform.InverseTransformDirection(mainBody.velocity);

		// Am I hitting the ground?
		if (useSuspension)
		{
			Ray ray = new Ray(myTransform.position, -myTransform.up);
			Debug.DrawRay(ray.origin, ray.direction, Color.red);

			if (Physics.Raycast(ray, out raycastHit, suspensionLength))
			{
				inAir = false;
				//			print(suspensionLength + ":" + raycastHit.distance);
				// Yes I am, push the car up a bit (At my own wheel position)
				mainBody.AddForceAtPosition(
					((myTransform.up * suspensionStrength) * (suspensionLength - raycastHit.distance)) * (1 - damping),
					myTransform.position);

				// Lateral friction (basically just push in the opposite direction to our sideways velocity)
				mainBody.AddForceAtPosition(
					myTransform.TransformDirection(new Vector3(-localVelocity.x * lateralFriction, 0, 0)),
					myTransform.position);
			}
			else
				inAir = true;
		}
		else
		{
			// Lateral friction (basically just push in the opposite direction to our sideways velocity)
			mainBody.AddForceAtPosition(
				myTransform.TransformDirection(new Vector3(-localVelocity.x * lateralFriction, 0, 0)),
				myTransform.position);
		}

	}

	//	private void OnCollisionExit(Collision other)
	//	{
	//		inAir = true;
	//	}
	//
	//	private void OnCollisionEnter(Collision other)
	//	{
	//		inAir = false;
	//	}

	public void LongditudinalForce(float force)
	{
		if (inAir)
		{
			return;
		}
		mainBody.AddForceAtPosition(myTransform.TransformDirection(0, 0, force), myTransform.position);
	}
}
