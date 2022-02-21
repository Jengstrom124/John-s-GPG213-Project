using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    private Rigidbody rb;

    //public float xScale = 25f;
    [Header("Adjust variance of the perlin value")]
    [Tooltip("Minus perlinScale from the perlin value to allow it to reach both above & below 0")]
    public float perlinScale;

    [Header("Adjust the turn scale")]
    public float wanderScale = 2f;

    [Header("Reference Only:")]
    public float perlin;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        perlin = Mathf.PerlinNoise(Time.time, 0.0f) - perlinScale;
        rb.AddRelativeTorque(0,perlin * wanderScale,0);
    }
}
