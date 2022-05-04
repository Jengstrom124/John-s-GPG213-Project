using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public Material regularMat;
        public Material actioningMat;

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
            regularMat = GetComponentInChildren<SkinnedMeshRenderer>().material;
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
                    rb.AddForceAtPosition(new Vector3(0, JumpForce, 0), transform.position);
                    GetComponentInChildren<SkinnedMeshRenderer>().material = actioningMat;
                    //transform.Rotate(-90, 0, 0);
                }
                else if (Input.GetKeyDown(SecondAction))
                {
                    curState = State.State_Charge;
                    GetComponentInChildren<SkinnedMeshRenderer>().material = actioningMat;
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
                    Action(InputActionPhase.Performed);
                    break;
                case State.State_Charge:
                    Action2(InputActionPhase.Performed);
                    break;
            }
        }

        void DriveShark(float accelarate, float steering)
        {
            rb.AddForceAtPosition(-TailPos.up * AccelSpeed * accelarate * Time.deltaTime,transform.position);
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

        public void Action(InputActionPhase aActionPhase)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                rb.AddRelativeForce(new Vector3(0, -SplashForce, 0));
                curState = State.State_Swimming;
                timer = 0;
                GetComponentInChildren<SkinnedMeshRenderer>().material = regularMat;
                //transform.Rotate(-180, 0, 0);
            }

            //rb.AddRelativeForce(new Vector3(0, -SplashForce, 0));
        }

        public void Action2(InputActionPhase aActionPhase)
        {
            rb.AddForceAtPosition(-TailPos.transform.up * ChargeForce * Time.deltaTime, transform.position);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public void Action3(InputActionPhase aActionPhase)
        {
            throw new System.NotImplementedException();
        }


        private void OnCollisionEnter(Collision collision)
        {
            if(curState == State.State_Charge)
            {
                curState = State.State_Swimming;
                GetComponentInChildren<SkinnedMeshRenderer>().material = regularMat;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}

