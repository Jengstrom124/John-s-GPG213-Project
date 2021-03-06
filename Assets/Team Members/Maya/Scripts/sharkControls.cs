using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace MayaStuff
{
    public class sharkControls : MonoBehaviour, IControllable, IReactsToInk
    {
        public Rigidbody sharkForce;

        //public Rigidbody sharkForward;
        public Transform sharkRotatePoint;

        public float speed;
        public Vector3 localVelocity;

        public float rotateSpeed;

        public bool swingLeft;
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
            //localVelocity = transform.InverseTransformDirection(sharkForce.velocity);
            Steer(Input.GetAxis("Horizontal"));
            Accelerate(Input.GetAxis("Vertical"));
            Action(InputActionPhase.Performed);
        }

        public void Steer(float input)
        {
                sharkForce.AddForceAtPosition(input*rotateSpeed*transform.TransformDirection(Vector3.right), transform.position, 0);
                sharkForce.AddRelativeTorque(new Vector3(0, (input*rotateSpeed), 0));
                sharkHead.DOLocalRotate(new Vector3(0, input * 15, 0), 1f, RotateMode.Fast);
        }

        public void Accelerate(float input)
        {
            if (input <0.5f)
            {
                input = 0.25f;
            }

            rotateSpeed = (input*5) + 12f;

            float swingSpeed = (input * 10) / 3;
            float maxSwingAngle = 16;
            sharkForce.AddForceAtPosition(input*speed*transform.TransformDirection(Vector3.forward), transform.position, 0);
            /*if (swingLeft)
            {
                tailBit1.DOLocalRotate(new Vector3(0, 0, maxSwingAngle), swingSpeed);
                tailBit2.DOLocalRotate(new Vector3(0, 0, maxSwingAngle), swingSpeed);
                tailBit3.DOLocalRotate(new Vector3(0, 0, -maxSwingAngle), swingSpeed);
                tailBit4.DOLocalRotate(new Vector3(0, 0, -maxSwingAngle), swingSpeed);
            }
            else
            {
                tailBit1.DOLocalRotate(new Vector3(0, 0, -maxSwingAngle), swingSpeed);
                tailBit2.DOLocalRotate(new Vector3(0, 0, -maxSwingAngle), swingSpeed);
                tailBit3.DOLocalRotate(new Vector3(0, 0, maxSwingAngle), swingSpeed);
                tailBit4.DOLocalRotate(new Vector3(0, 0, maxSwingAngle), swingSpeed);
            }

            if (Math.Abs(tailBit1.localRotation.eulerAngles.z - 16f) < 1f)
            {
                swingLeft = false;
            }
            else if(Math.Abs(tailBit1.localRotation.eulerAngles.z - -16f) < 1f)
            {
                swingLeft = true;
            }*/
        }

        public void Reverse(float input)
        {

        }

        public void Action(InputActionPhase aActionPhase)
        {
            speed = 100;
        }

        public void Action2(InputActionPhase aActionPhase)
        {

        }

        public void Action3(InputActionPhase aActionPhase)
        {
            speed = 2;
        }

        public void ChangeInkedState(bool isInked)
        {
            if (isInked)
                speed = 2f;
        }
    }

}