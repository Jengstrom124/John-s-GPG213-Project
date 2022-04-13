using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

        public Transform tailBit1;
        public Transform tailBit2;
        public Transform tailBit3;
        public Transform tailBit4;
        public Transform sharkHead;

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
                //sharkForce.AddRelativeTorque(new Vector3(0, rotateSpeed, 0));
                sharkForce.AddForceAtPosition(input*rotateSpeed*transform.TransformDirection(Vector3.right), transform.position, 0);
                sharkForce.AddRelativeTorque(new Vector3(0, (input*rotateSpeed), 0));
                sharkHead.transform.localRotation = Quaternion.Euler(0, input*rotateSpeed*2, (0));



            }

            else if (Input.GetKey(KeyCode.A))
            {
                //sharkForce.AddForceAtPosition(new Vector3(-rotateSpeed, 0, (-localVelocity.z * rotateSpeed)), sharkRotatePoint.position);
                sharkForce.AddForceAtPosition(-input*rotateSpeed*transform.TransformDirection(Vector3.left), transform.position, 0);
                sharkForce.AddRelativeTorque(new Vector3(0, (-input*-rotateSpeed), 0));
                sharkHead.transform.localRotation = Quaternion.Euler(0, (-input*-rotateSpeed*2), 0);



                //sharkSteering.AddRelativeForce(Vector3.right * rotateSpeed);
                //sharkSteering.AddTorque(Vector3.right * rotateSpeed);
            }
            else
            {
                sharkHead.transform.localRotation = Quaternion.Lerp(quaternion.identity, new Quaternion(0, input,0, 0), 0);
                //sharkHead.transform.localRotation = quaternion.identity;
            }
        }

        public void Accelerate(float input)
        {
            if (input <0.5f)
            {
                input = 0.25f;
            }

            rotateSpeed = (input*2) + 3;
            sharkForce.AddForceAtPosition(input*speed*transform.TransformDirection(Vector3.forward), transform.position, 0);
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