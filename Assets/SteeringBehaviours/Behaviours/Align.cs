using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBase
{
    Rigidbody rb;
	Neighbours neighbours;

	public float force = 2f;

    private void Start()
    {
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
	{
		//rb.AddTorque(CalculateMove(neighbours.neighbours) * force);

		rb.AddTorque(Vector3.Cross(transform.forward, CalculateMove(neighbours.neighbours)) * force);
	}

	public override Vector3 CalculateMove(List<GameObject> neighbours)
	{
		if (neighbours.Count == 0)
        {
			return transform.forward;
        }

		Vector3 alignmentMove = Vector3.zero;

		// Average of all neighbours directions
		foreach (GameObject neighbour in neighbours)
		{
			alignmentMove += neighbour.transform.forward;
		}

		alignmentMove /= neighbours.Count;

		return alignmentMove;
	}

}
