using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : MonoBehaviour
{
	Rigidbody rb;
	Neighbours neighbours;

	public float speed = 5f;

	private void Start()
	{
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		// Some are Torque, some are Force
		rb.AddForce(CalculateMove(neighbours.neighbours));
	}

	public Vector3 CalculateMove(List<GameObject> neighbours)
	{
		if (neighbours.Count == 0)
        {
			return Vector3.zero;
        }

		Vector3 cohesionMove = Vector3.zero;

		// Average of all neighbours positions
		foreach (GameObject neighbour in neighbours)
		{
			if(Vector3.Distance(transform.position, neighbour.transform.position) > 2f)
            {
				cohesionMove += transform.InverseTransformPoint(neighbour.transform.position);
            }
		}

		cohesionMove /= neighbours.Count;

		return cohesionMove;
	}
}
