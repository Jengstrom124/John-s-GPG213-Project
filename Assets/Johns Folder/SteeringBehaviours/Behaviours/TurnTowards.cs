using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTowards : MonoBehaviour
{
    Rigidbody rb;

    [Header("Target Values")]
    public Transform target;
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
        targetPos = transform.InverseTransformPoint(target.position);
        targetXPos = targetPos.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddTorque(new Vector3(0, targetXPos, 0) * turnMultiplier);
    }
}
