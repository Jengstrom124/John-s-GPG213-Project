using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    private Rigidbody rb;
    
    public float xScale = 1.0f;
    public float perlin;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    

    // Update is called once per frame
    void Update()
    {
        perlin = Mathf.PerlinNoise(Time.time * xScale, 0.0f);
        //Vector3 pos = transform.position;
        //pos.y = height;
        //transform.position = pos;
        
        rb.AddRelativeTorque(0,perlin,0);
    }
}
