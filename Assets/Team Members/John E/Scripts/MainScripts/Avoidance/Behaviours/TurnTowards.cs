using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowards : MonoBehaviour
{
    Rigidbody rb;

    [Header("Setup: ")]
    public Transform myTransform;
    public float turnMultiplier = 0.05f;
    MoveForwards moveForwards;

    [Header("Reference Only")]
    public Transform target;
    public Vector3 destinationTarget;
    Vector3 targetDirection;
    bool runAway;
    public float angle;

    PathTracker pathTracker;
    Neighbours neighbours;

    private void Awake()
    {
        pathTracker = GetComponent<PathTracker>();
        moveForwards = GetComponent<MoveForwards>();
        neighbours = GetComponent<Neighbours>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        neighbours.newNeighbourEvent += CheckForPredator;
        neighbours.neighbourLeaveEvent += PredatorOutOfSight;

        pathTracker.newTargetAssignedEvent += SetDestinationTarget;
    }
    private void Update()
    {
        //For using a transform reference
        if(target != null)
        {
            if(runAway)
            {
                //Opposite direction of 'target' - using to escape predators
                targetDirection = -target.position;
            }
            else
            {
                //Head towards target
                targetDirection = transform.InverseTransformPoint(target.position);
                //targetDirection = target.position - myTransform.position;
            }
        }

        //Raw Vector position
        if(destinationTarget != Vector3.zero && target == null)
        {
            targetDirection = transform.InverseTransformPoint(destinationTarget);
            //targetDirection = destinationTarget - myTransform.position;
        }

        //angle = Vector3.Angle(targetDirection, myTransform.forward);
        angle = targetDirection.x;

        //Include speed change based on wider angle to stop circuling around a target
        if(angle < 10f && Vector3.Distance(destinationTarget, myTransform.position) < 10f && !runAway)
        {
            if(moveForwards != null)
            {
                moveForwards.speed = Mathf.Clamp(moveForwards.speed = angle, 0, moveForwards.maxSpeed);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null || destinationTarget != Vector3.zero)
        {
            rb.AddRelativeTorque(new Vector3(0, angle, 0) * turnMultiplier);
        }
    }

    void EscapePredator(bool runBool, Transform predator)
    {
        runAway = runBool;
        target = predator;
    }

    void SetDestinationTarget(Vector3 currentTarget)
    {
        destinationTarget = currentTarget;
    }

    void CheckForPredator(GameObject other)
    {
        if (other.GetComponent<IPredator>() != null)
        {
            EscapePredator(true, other.transform);
        }
    }

    void PredatorOutOfSight(GameObject other)
    {
        if (other.GetComponent<IPredator>() != null)
        {
            EscapePredator(false, null);
        }
    }
}
