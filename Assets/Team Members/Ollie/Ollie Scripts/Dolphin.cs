using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Ollie
{
    public class Dolphin : OllieVehicleBase
    {
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

        private void Update()
        {
            if (rb.velocity.y > 0)
            {
                //up
                model.localRotation = Quaternion.Euler(0,90,-45);
            }
            else if (rb.velocity.y < 0)
            {
                model.localRotation = Quaternion.Euler(0,90,45);
            }
            else
            {
                model.localRotation = Quaternion.Euler(0,90,0);
            }
            
            if (!jumping)
            {
                WaterCheck();
            }
        }

        public override void Action()
        {
            JumpOut();
        }

        public override void Action2()
        {
            SpeedBoost();
        }

        public override void Action3()
        {
            
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
            if (!jumping)
            {
                jumping = true;
                StartCoroutine(JumpCooldownCoroutine());
                //do jump code
                //turn off y constraint
                //rotate on y axis, maybe 45 degrees up?
                //apply force and lock controls?
                
                //rb.constraints = RigidbodyConstraints.None;
                //rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
                
                //up
                
                //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                rb.drag = 5;
                rb.angularDrag = 1;
                rb.AddForce(0,jumpHeight,jumpDistance, ForceMode.Impulse);
            }
            else
            {
                print("dolphin's can't double jump, dummy");
            }
        }
        
        //turn off drags when you're flying above water
        //use IReactsToWater to check if in water
        //fix capsule collider

        IEnumerator JumpCooldownCoroutine()
        {
            print("dolphin go yeet");
            yield return new WaitForSeconds(1.5f);
            //down
            //yield return new WaitForSeconds(1f);
            //normal
            jumping = false;
            rb.drag = originalDrag;
            rb.angularDrag = originalAngularDrag;
        }
        
        void SpeedBoost()
        {
            if (!boosting)
            {
                StartCoroutine(SpeedBoostCoroutine());
            }
        }

        IEnumerator SpeedBoostCoroutine()
        {
            //doubles speed for 1.5 seconds
            //prevents reapplication for 3 seconds after deactivation
            boosting = true;
            forwardSpeed = forwardSpeed * 2;
            yield return new WaitForSeconds(1.5f);
            forwardSpeed = forwardSpeed / 2;
            yield return new WaitForSeconds(3f);
            boosting = false;
        }
    }
}