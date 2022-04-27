using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ollie
{
    public class OllieShark : OllieVehicleBase
    {
        private void Start()
        {
            rb = GetComponentInChildren<Rigidbody>();
            capsuleCollider = GetComponentInChildren<CapsuleCollider>();
            groundSpeed = 22.5f;
            forwardSpeed = 35;
            turnSpeed = 10;
            car = this.gameObject;
        }

        public override void Action()
        {
            if (IsServer)
            {

            }
        }

        public override void Action2()
        {
            if (IsServer)
            {
                SpeedBoost();
            }
        }

        public override void Action3()
        {
            if (IsServer)
            {

            }
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
