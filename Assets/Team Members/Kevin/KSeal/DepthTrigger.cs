using System;
using System.Collections;
using System.Collections.Generic;
using Kevin;
using Unity.VisualScripting;
using UnityEngine;

public class DepthTrigger : MonoBehaviour
{
    public GameObject sealPrefab;
    
    void OnTriggerEnter(Collider other)
    {
        Seal sealScript = sealPrefab.GetComponent<Kevin.Seal>();
        //sealScript.accelerationSpeed = 5f;
        sealScript.currentSteeringMax = sealScript.steeringMax;
        sealScript.sealRigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}
