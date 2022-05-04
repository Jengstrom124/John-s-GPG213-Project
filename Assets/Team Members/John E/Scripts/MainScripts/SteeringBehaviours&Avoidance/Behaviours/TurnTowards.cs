using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowards : MonoBehaviour
{
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
    float defaultTurnMultiplier;

    PathTracker pathTracker;
    Neighbours neighbours;
    Rigidbody rb;

    private void Awake()
    {
        pathTracker = GetComponent<PathTracker>();
        moveForwards = GetComponent<MoveForwards>();
        neighbours = GetComponent<Neighbours>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(neighbours != null)
        {
            neighbours.seePredatorEvent += CheckForPredator;
        }

        pathTracker.newTargetAssignedEvent += SetDestinationTarget;

        defaultTurnMultiplier = turnMultiplier;
    }
    private void Update()
    {
        FindTransformDirection();

        FindRawPositionDirection();

        CalculateAngle();
    }

    void FixedUpdate()
    {
        if(target != null || destinationTarget != Vector3.zero)
        {
            rb.AddRelativeTorque(new Vector3(0, angle, 0) * turnMultiplier);
        }
    }

    //Listen for when a target is assigned and set that as our look at target
    void SetDestinationTarget(Vector3 currentTarget)
    {
        destinationTarget = currentTarget;
    }

    //Transform Specific Direction Calculations
    void FindTransformDirection()
    {
        //For using a transform reference as a target
        if (target != null)
        {
            if (runAway)
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
    }

    //Raw Vector Position Direction Calculations
    void FindRawPositionDirection()
    {
        //If a direction  has been assigned + using target null here to not overwrite running from a shark
        if (destinationTarget != Vector3.zero && target == null)
        {
            targetDirection = transform.InverseTransformPoint(destinationTarget);
            //targetDirection = destinationTarget - myTransform.position;
        }
    }

    //Calculate angle to apply in torque
    void CalculateAngle()
    {
        //angle = Vector3.Angle(targetDirection, myTransform.forward);
        angle = targetDirection.x;

        //Include speed change based on wider angle to stop circuling around a target
        if (angle < 10f && Vector3.Distance(destinationTarget, myTransform.position) < 10f && !runAway)
        {
            if (moveForwards != null)
            {
                moveForwards.speed = Mathf.Clamp(moveForwards.speed = angle, 0, moveForwards.maxSpeed);
            }
        }


    }

    #region Predator Specific Implementation
    void CheckForPredator(GameObject other)
    {
        if(other == null)
        {
            EscapePredator(false, null);
            turnMultiplier = defaultTurnMultiplier;
        }
        else if(other.GetComponent<IPredator>() != null)
        {
            EscapePredator(true, other.transform);
            turnMultiplier /= 4f;
        }
    }

    void EscapePredator(bool runBool, Transform predator)
    {
        runAway = runBool;
        target = predator;
    }
    #endregion
}
