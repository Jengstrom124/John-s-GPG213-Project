using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ollie
{
    public class Dolphin : OllieVehicleBase
    {
        public AudioSource splash;
        public AudioSource boost;
        private void Start()
        {
            rb = GetComponentInChildren<Rigidbody>();
            capsuleCollider = GetComponentInChildren<CapsuleCollider>();
            // bumPoint = GetComponentInChildren<Transform>();
            // stomach = GetComponentInChildren<Stomach>();
            groundSpeed = 22.5f;
            forwardSpeed = 35;
            turnSpeed = 10;
            car = this.gameObject;
            jumping = false;
            jumpHeight = 20;
            jumpDistance = 00;
            originalConstraints = rb.constraints;
            originalDrag = rb.drag;
            originalAngularDrag = rb.angularDrag;
        }
        
        private void FixedUpdate()
        {
            if (IsServer)
            {
                if (rb.velocity.y > 0)
                {
                    //up
                    model.localRotation = Quaternion.Euler(-45, 0, 0);
                }
                else if (rb.velocity.y < 0)
                {
                    model.localRotation = Quaternion.Euler(45, 0, 0);
                }
                else
                {
                    model.localRotation = Quaternion.Euler(0, 0, 0);
                }

                if (!jumping)
                {
                    WaterCheck();
                }
            }
        }

        public override void Action(InputActionPhase aActionPhase)
        {
            if (IsServer)
            {
                JumpOut();
            }

            if (IsClient)
            {
                splash.Play();
            }
        }

        public override void Action2(InputActionPhase aActionPhase)
        {
            if (IsServer)
            {
                SpeedBoost();
            }

            if (IsClient)
            {
                boost.Play();
            }
        }

        public override void Action3(InputActionPhase aActionPhase) // test to shit out fish
        //player controller uses "wasPressed" so shits out heaps of fish per press rather than just one
        //but it works!
        {
            if (IsServer && IsOwner)
            {
                stomach.fishContainer.PopFishFromGuts(1); //change this from 1 to edibleInfo.amount at some point
                //print("after shitting, count equals " +stomach.fishContainer.totalFoodAmount);
            }
        }
        
        public override void WaterCheck()
        {
            if (IsWet)
            {
                rb.constraints = originalConstraints;
                //print("wet" + this);
            }

            if (!IsWet)
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
                //print("not wet" + this);
            }
        }

        void JumpOut()
        {
            if (IsServer)
            {
                if (!jumping && IsWet)
                {
                    jumping = true;
                    StartCoroutine(JumpCooldownCoroutine());
                    rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
                    rb.drag = 5;
                    rb.angularDrag = 3;
                    rb.AddForce(0, jumpHeight, jumpDistance, ForceMode.Impulse);
                }
                else
                {
                    //print("dolphin's can't double jump, dummy");
                }
            }
        }

        IEnumerator JumpCooldownCoroutine()
        {
            //print("dolphin go yeet");
            yield return new WaitForSeconds(0.75f);
            rb.AddForce(0,-jumpHeight,jumpDistance, ForceMode.Impulse);
            jumping = false;
            yield return new WaitForSeconds(1.5f);
            rb.drag = originalDrag;
            rb.angularDrag = originalAngularDrag;
        }
        
        void SpeedBoost()
        {
            if (IsServer)
            {
                if (!boosting)
                {
                    StartCoroutine(SpeedBoostCoroutine());
                }
            }
        }

        IEnumerator SpeedBoostCoroutine()
        {
            //doubles speed for 1.5 seconds
            //prevents reapplication for 3 seconds after deactivation
            //print("shark go zoom");
            boosting = true;
            forwardSpeed = forwardSpeed * 2;
            yield return new WaitForSeconds(1.5f);
            forwardSpeed = forwardSpeed / 2;
            yield return new WaitForSeconds(3f);
            boosting = false;
        }
    }
}