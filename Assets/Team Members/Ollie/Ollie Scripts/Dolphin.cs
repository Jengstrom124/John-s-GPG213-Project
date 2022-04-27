using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

                if (rb.position.y < 10f && !jumping)
                {
                    rb.constraints = originalConstraints;
                }
            }
        }

        public override void Action()
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

        public override void Action2()
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

        public override void Action3()
        {
            if (IsServer)
            {
                
            }
        }
        
        public override void WaterCheck()
        {
            if (IsWet)
            {
                rb.constraints = originalConstraints;
                print("wet");
            }

            if (!IsWet)
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
                print("not wet");
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
                    print("dolphin's can't double jump, dummy");
                }
            }
        }

        IEnumerator JumpCooldownCoroutine()
        {
            print("dolphin go yeet");
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
            print("shark go zoom");
            boosting = true;
            forwardSpeed = forwardSpeed * 2;
            yield return new WaitForSeconds(1.5f);
            forwardSpeed = forwardSpeed / 2;
            yield return new WaitForSeconds(3f);
            boosting = false;
        }
    }
}