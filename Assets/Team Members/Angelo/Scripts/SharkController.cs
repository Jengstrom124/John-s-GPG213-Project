using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

namespace AnGelloStuff
{
    public class SharkController : NetworkBehaviour, IControllable, IPredator
    {
        public int AccelSpeed;
        public int steeringSpeed;
        public int JumpForce;
        public int SplashForce;
        public int ChargeForce;
        public int foodCounter;
        float AccelInput;
        float SteerInput;
        float ReverseInput;

        float Hungertimer;
        public float SetStarvationTime = 5;

        public KeyCode FirstAction;
        public KeyCode SecondAction;

        public Transform TailPos;
        public Vector3 Bum;
        private float timer;

        public Material regularMat;
        public Material actioningMat;
        private FishContainer fishCounter;

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
            fishCounter = GetComponent<FishContainer>();
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
                    
                        rb.AddForceAtPosition(new Vector3(0, JumpForce, 0), transform.position);
                    
                    
                        curState = State.State_Splashdown;
                        GetComponentInChildren<SkinnedMeshRenderer>().material = actioningMat;
                    
                    //transform.Rotate(-90, 0, 0);
                }
                else if (Input.GetKeyDown(SecondAction))
                {
                    
                        curState = State.State_Charge;
                        GetComponentInChildren<SkinnedMeshRenderer>().material = actioningMat;
                    
                }
            }

            Hungertimer += Time.deltaTime;

            if (Hungertimer >= SetStarvationTime & foodCounter > -5)
            {
                Hungertimer = 0;
                foodCounter -= 1;
                ScaleShark();
            }
        }

        private void FixedUpdate()
        {
            ScaleShark();
            
                switch (curState)
                {
                    case State.State_Swimming:
                        DriveShark(AccelInput + ReverseInput, SteerInput);
                        break;
                    case State.State_Splashdown:
                        //Action();
                        break;
                    case State.State_Charge:
                        //Action2();
                        break;
                }
             
        }

        void DriveShark(float accelarate, float steering)
        {
            rb.AddForceAtPosition(-TailPos.up * AccelSpeed * accelarate * Time.deltaTime,transform.position);
            //rb.AddForceAtPosition(TailPos.forward * AccelSpeed * accelarate, transform.position);
            rb.AddRelativeTorque(0, steering * steeringSpeed * (AccelInput + ReverseInput), 0);
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

        public void Action2(InputActionPhase aInputActionPhase)
        {
            rb.AddForceAtPosition(-TailPos.transform.up * ChargeForce * Time.deltaTime, transform.position);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public void Action3(InputActionPhase aInputActionPhase)
        {
            throw new System.NotImplementedException();
        }


        private void OnCollisionEnter(Collision collision)
        {
            
                if (curState == State.State_Charge)
                {
                    curState = State.State_Swimming;
                    GetComponentInChildren<SkinnedMeshRenderer>().material = regularMat;
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            IEdible edible = other.GetComponent<IEdible>();
            FishBase fish = other.GetComponent<FishBase>();

            if(edible != null)
            {
                fishCounter.AddToStomach(fish);
            }

            if(fish != null)
            {
                foodCounter = fishCounter.totalFoodAmount;
                ScaleShark();
            }
        }

        public Vector3 GetBumPosition()
        {
            return Bum;
        }

        public void GetEaten(IPredator eatenBy)
        {
            throw new System.NotImplementedException();
        }

        public EdibleInfo GetInfo()
        {
            return new EdibleInfo();
        }

        private void ScaleShark()
        {
            transform.localScale = new Vector3(1, 1, 1) * ((0.1f * foodCounter) + 1);
        }
    }
}

