using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Tom
{
    public class CameraFollow : NetworkBehaviour
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