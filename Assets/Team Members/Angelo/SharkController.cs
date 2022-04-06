using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnGelloStuff
{
    public class SharkController : MonoBehaviour
    {
        public int AccelSpeed;
        public int steeringSpeed;

        public Transform TailPos;

        Rigidbody rb;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            DriveShark(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        }

        void DriveShark(float accelarate, float steering)
        {
            rb.AddRelativeForce(TailPos.forward * AccelSpeed * accelarate);
            //rb.AddForceAtPosition(TailPos.forward * AccelSpeed * accelarate, transform.position);
            rb.AddRelativeTorque(0, steering, 0);
        }
    }
}

