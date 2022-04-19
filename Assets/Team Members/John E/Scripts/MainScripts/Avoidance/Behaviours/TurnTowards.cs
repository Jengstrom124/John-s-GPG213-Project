using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowards : MonoBehaviour
{
    Rigidbody rb;

    [Header("Target Values")]
    public Transform target;
    public Vector3 destinationTarget;
    public float turnMultiplier = 0.05f;

    [Header("Reference Only")]
    Vector3 targetPos;
    bool runAway;
    public float targetXPos;

    //TEST
    PathTracker pathTracker;
    FishModel fish;

    private void Awake()
    {
        fish = GetComponent<FishModel>();
        pathTracker = GetComponent<PathTracker>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        fish.runFromPredatorEvent += EscapePredator;
        //fish.onDestinationAssignedEvent += SetDestinationTarget;

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
                targetPos = -target.position;
            }
            else
            {
                //Head towards target
                targetPos = transform.InverseTransformPoint(target.position);
            }
        }

        //Raw Vector position
        if(destinationTarget != Vector3.zero && target == null)
        {
            targetPos = transform.InverseTransformPoint(destinationTarget);
        }

        //Change to vector
        targetXPos = targetPos.x;

        //Include speed change based on wider angle to stop circuling around a target
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null || destinationTarget != Vector3.zero)
        {
            rb.AddRelativeTorque(new Vector3(0, targetXPos, 0) * turnMultiplier);
        }
    }

    void EscapePredator(bool runBool, Transform predator)
    {
        runAway = runBool;
        target = predator;
    }

    void SetDestinationTarget(Vector3 currentTarget)
    {
        Debug.Log(currentTarget);
        destinationTarget = currentTarget;
    }
}
