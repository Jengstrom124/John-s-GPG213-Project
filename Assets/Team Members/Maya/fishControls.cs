using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MayaStuff
{
    public class fishControls : MonoBehaviour, IControllable
    {
        public Rigidbody sharkForce;

        //public Rigidbody sharkForward;
        public Transform sharkRotatePoint;

        public float speed;
        public Vector3 localVelocity;

        public float rotateSpeed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            localVelocity = transform.InverseTransformDirection(sharkForce.velocity);
            Steer(Input.GetAxis("Horizontal"));
            Accelerate(Input.GetAxis("Vertical"));
            Action();
        }

        public void Steer(float input)
        {
            if (Input.GetKey(KeyCode.D))
            {
                //sharkForce.AddForceAtPosition(new Vector3(rotateSpeed, 0, (-localVelocity.z * rotateSpeed)), sharkRotatePoint.position);
                //sharkSteering.AddTorque(Vector3.left * rotateSpeed);
                //sharkSteering.AddRelativeTorque(Vector3.left * rotateSpeed);
                sharkForce.AddRelativeTorque(new Vector3(0, rotateSpeed, 0));
            }

            if (Input.GetKey(KeyCode.A))
            {
                //sharkForce.AddForceAtPosition(new Vector3(-rotateSpeed, 0, (-localVelocity.z * rotateSpeed)), sharkRotatePoint.position);
                sharkForce.AddRelativeTorque(new Vector3(0, -rotateSpeed, 0));
                //sharkSteering.AddRelativeForce(Vector3.right * rotateSpeed);
                //sharkSteering.AddTorque(Vector3.right * rotateSpeed);
            }
        }

        public void Accelerate(float input)
        {
            sharkForce.AddRelativeForce(Vector3.forward * speed);
        }

        public void Reverse(float input)
        {

        }

        public void Action()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 100;
            }
            else
                speed = 25;
        }

        public void Action2()
        {

        }

        public void Action3()
        {

        }
    }

}