using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cam
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject controlledThing;

        // Update is called once per frame
        void Update()
        {
            // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
            IControllable controllable = controlledThing.GetComponentInChildren<IControllable>();

            controllable.Steer(0f);
            if (InputSystem.GetDevice<Keyboard>().aKey.wasPressedThisFrame)
            {
                // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                controllable.Steer(-1f);
            }
            if (InputSystem.GetDevice<Keyboard>().dKey.wasPressedThisFrame)
            {
                // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                controllable.Steer(1f);
            }
            if (InputSystem.GetDevice<Keyboard>().wKey.wasPressedThisFrame)
            {
                // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                controllable.Accelerate(1f);
            }
            if (InputSystem.GetDevice<Keyboard>().sKey.wasPressedThisFrame)
            {
                // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                controllable.Reverse(1f);
            }
            if (InputSystem.GetDevice<Keyboard>().fKey.wasPressedThisFrame)
            {
                // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                controllable.Action();
            }
            if (InputSystem.GetDevice<Keyboard>().eKey.wasPressedThisFrame)
            {
                // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                controllable.Action2();
            }
            if (InputSystem.GetDevice<Keyboard>().qKey.wasPressedThisFrame)
            {
                // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
                controllable.Action3();
            }        
        }
    }
}