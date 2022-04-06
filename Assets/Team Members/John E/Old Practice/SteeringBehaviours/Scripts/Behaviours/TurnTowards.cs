using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowards : MonoBehaviour
{
    Rigidbody rb;

    [Header("Target Values")]
    public Vector3 target;
    public float turnMultiplier = 0.05f;

    [Header("Reference Only")]
    Vector3 targetPos;
    public float targetXPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        //targetPos = transform.TransformVector(target);
        targetXPos = target.x;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != Vector3.zero)
        {
            Debug.Log(targetXPos);
            rb.AddRelativeTorque(new Vector3(0, targetXPos, 0) * turnMultiplier);
        }
    }

    /*
    float TurnDirection(Vector3 pos)
    {
        targetPos = transform.InverseTransformPoint(target.position);
        targetXPos = targetPos.x;

        return targetXPos;
    }
    */
}
