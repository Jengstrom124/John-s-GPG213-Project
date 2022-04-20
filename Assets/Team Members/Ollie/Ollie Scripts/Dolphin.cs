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
                model.localRotation = Quaternion.Euler(0,90,-45);
                //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
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
            yield return new WaitForSeconds(0.75f);
            model.localRotation = Quaternion.Euler(0,90,45);
            // if (rb.transform.position.y > 10)
            // {
            //     rb.mass = 5;
            //     yield return null;
            // }
            // else
            // {
            //     rb.mass = 1;
            // }
            yield return new WaitForSeconds(0.33f);
            model.localRotation = Quaternion.Euler(0,90,0);
            jumping = false;
            //rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ;
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