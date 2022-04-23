using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kevin
{
    public class Eagle : MonoBehaviour
    {
        public Transform currentLocation;
        
        public void Update()
        {
            transform.position = new Vector3(Random.Range(0f, 256f), transform.position.y, Random.Range(0f, 256f));
        }
    }
}

