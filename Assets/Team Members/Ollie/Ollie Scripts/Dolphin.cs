using System.Collections;
using System.Collections.Generic;
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
            jumpAngle = 45;
            jumpHeight = 10;
            jumpDistance = 10;
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
                rb.constraints = RigidbodyConstraints.None;
                rb.constraints = RigidbodyConstraints.FreezeRotationY;
                rb.constraints = RigidbodyConstraints.FreezeRotationZ;
                rb.AddRelativeTorque(jumpAngle,0,0);
                rb.constraints = RigidbodyConstraints.FreezeRotationX;
                rb.AddForce(0,jumpHeight,jumpDistance, ForceMode.Acceleration);
            }
            else
            {
                print("dolphin's can't double jump, dummy");
            }
        }

        IEnumerator JumpCooldownCoroutine()
        {
            print("dolphin go yeet");
            yield return new WaitForSeconds(3f);
            jumping = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationX;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            rb.constraints = RigidbodyConstraints.FreezeRotationZ;
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