using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Cam
{
    public class PlayerController : MonoBehaviour
    {
	    // This is Go purely for inspector dragdropping. It should be IControllable if Unity supported that
        // public GameObject controlledThing;
        // IControllable     controllable;
        //
        // void Start()
        // {
        //     // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //     controllable = controlledThing.GetComponentInChildren<IControllable>();
        //
        //     DefaultControls defaultControls = new DefaultControls();
        //     defaultControls.InGame.Action1.performed += Action1_Onperformed;
        //     defaultControls.InGame.Action2.performed += Action2_Onperformed;
        //     
        //     defaultControls.InGame.Controls.performed += Controls_Onperformed;
        //     
        //     defaultControls.InGame.Enable();
        //     defaultControls.InMenu.Disable();
        // }
        //
        // void Controls_Onperformed(InputAction.CallbackContext obj)
        // {
        //     Debug.Log(obj.ReadValue<float>());
        // }
        //
        // void Action1_Onperformed(InputAction.CallbackContext obj)
        // {
        //     if (obj.phase == InputActionPhase.Performed)
        //     {
        //         controllable.Action();
        //     }
        // }
        // void Action2_Onperformed(InputAction.CallbackContext obj)
        // {
        //     controllable.Action2();
        // }
        //
        //
        //
        // // Update is called once per frame
        // void Update()
        // {
        //
        //
        //     
        //     controllable.Steer(0f);
        //     if (InputSystem.GetDevice<Keyboard>().aKey.wasPressedThisFrame)
        //     {
        //         // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //         controllable.Steer(-1f);
        //     }
        //     if (InputSystem.GetDevice<Keyboard>().dKey.wasPressedThisFrame)
        //     {
        //         // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //         controllable.Steer(1f);
        //     }
        //     if (InputSystem.GetDevice<Keyboard>().wKey.wasPressedThisFrame)
        //     {
        //         // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //         controllable.Accelerate(1f);
        //     }
        //     if (InputSystem.GetDevice<Keyboard>().sKey.wasPressedThisFrame)
        //     {
        //         // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //         controllable.Reverse(1f);
        //     }
        //     if (InputSystem.GetDevice<Keyboard>().fKey.wasPressedThisFrame)
        //     {
        //         // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //         controllable.Action();
        //     }
        //     if (InputSystem.GetDevice<Keyboard>().eKey.wasPressedThisFrame)
        //     {
        //         // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //         controllable.Action2();
        //     }
        //     if (InputSystem.GetDevice<Keyboard>().qKey.wasPressedThisFrame)
        //     {
        //         // Can't drag interfaces directly in the inspector, so get at it from a component/GameObject reference instead
        //         controllable.Action3();
        //     }        
        // }
    }
}