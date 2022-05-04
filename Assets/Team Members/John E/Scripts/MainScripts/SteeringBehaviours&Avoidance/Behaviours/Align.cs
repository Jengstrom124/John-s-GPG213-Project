using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBase
{
    Rigidbody rb;
	Neighbours neighbours;

	[Header("Align Forwards Direction With Group Forces")]
	public float force = 5f;

	[Header("Do you care about aligning with the player?")]
	public bool alignWithPlayer = true;
	
	[Header("Reference Only/Ignore: ")]
	public GameObject currentPlayerFish;

    private void Awake()
    {
		neighbours = GetComponent<Neighbours>();
		rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
		John.JohnRTSTestController.Instance.playerFishSelectedEvent += UpdatePlayerFish;
    }

    void FixedUpdate()
	{
		rb.AddTorque(Vector3.Cross(transform.forward, CalculateMove(neighbours.neighboursList)) * force);
	}

	public override Vector3 CalculateMove(List<GameObject> neighbours)
	{
		if (neighbours.Count == 0)
        {
			return transform.forward;
        }

		Vector3 alignmentMove = Vector3.zero;

		if(currentPlayerFish != null)
        {
			if (neighbours.Contains(currentPlayerFish) && alignWithPlayer)
			{
				alignmentMove = currentPlayerFish.transform.forward;
			}
		}
		else
        {
			// Average of all neighbours directions
			foreach (GameObject neighbour in neighbours)
			{
				alignmentMove += neighbour.transform.forward;
			}
		}

		alignmentMove /= neighbours.Count;

		return alignmentMove;
	}

	void UpdatePlayerFish(GameObject playerFish)
    {
		currentPlayerFish = playerFish;
    }
}
