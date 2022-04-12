using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tom
{
    public class CameraFollow : MonoBehaviour
    {
        public Camera cam;
        public Transform target;
        public Vector3 offset;
    
        private void Update()
        {
            if (target != null)
            {
                cam.transform.position = target.position + offset;
                cam.transform.LookAt(target);
            }
        }
    }
}