using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnGelloStuff
{
    public class SharkController : MonoBehaviour, IControllable
    {
        public int AccelSpeed;
        public int steeringSpeed;
        public int JumpForce;
        public int SplashForce;
        public int ChargeForce;
        float AccelInput;
        float SteerInput;
        float ReverseInput;

        public KeyCode FirstAction;
        public KeyCode SecondAction;

        public Transform TailPos;
        private float timer;

        public Material mat;

        Rigidbody rb;

        enum State {
            State_Swimming,
            State_Splashdown,
            State_Charge
        };

        State curState;

        

        // Start is called before the first frame update
        void Start()
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = mat;
            
            curState = State.State_Swimming;
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            Accelerate(Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1));
            Reverse(Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0));
            Steer(Input.GetAxis("Horizontal"));

            if(curState == State.State_Swimming)
            {
                if (Input.GetKeyDown(FirstAction))
                {
                    curState = State.State_Splashdown;
                    rb.AddRelativeForce(new Vector3(0, JumpForce, 0));
                    //transform.Rotate(-90, 0, 0);
                }
                else if (Input.GetKeyDown(SecondAction))
                {
                    curState = State.State_Charge;
                }
            }          
        }

        private void FixedUpdate()
        {
            switch (curState)
            {
                case State.State_Swimming:
                    DriveShark(AccelInput + ReverseInput, SteerInput);
                    break;
                case State.State_Splashdown:
                    Action();
                    break;
                case State.State_Charge:
                    Action2();
                    break;
            }
        }

        void DriveShark(float accelarate, float steering)
        {
            rb.AddRelativeForce(new Vector3(0,0, AccelSpeed * accelarate));
            //rb.AddForceAtPosition(TailPos.forward * AccelSpeed * accelarate, transform.position);
            rb.AddRelativeTorque(0, steering * steeringSpeed, 0);
        }

        public void Steer(float input)
        {
            SteerInput = input;
        }

        public void Accelerate(float input)
        {
            AccelInput = input;
        }

        public void Reverse(float input)
        {
            ReverseInput = input;
        }

        public void Action()
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                rb.AddRelativeForce(new Vector3(0, -SplashForce, 0));
                curState = State.State_Swimming;
                timer = 0;
                //transform.Rotate(-180, 0, 0);
            }

            //rb.AddRelativeForce(new Vector3(0, -SplashForce, 0));
        }

        public void Action2()
        {
            rb.AddForceAtPosition(new Vector3(0, 0, ChargeForce), transform.position);
        }

        public void Action3()
        {
            throw new System.NotImplementedException();
        }


        private void OnCollisionEnter(Collision collision)
        {
            if(curState == State.State_Charge)
            {
                curState = State.State_Swimming;
            }
        }
    }



    /*Little Networking
     * Model, View
     * Splashdown
     * Properly Hover
     * IsClient and IsServer functions for the shark
     * Animation controller
    */
}

