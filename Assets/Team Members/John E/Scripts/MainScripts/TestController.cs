using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace John
{
    public class TestController : ManagerBase<TestController>
    {
        List<FishModel> activeFish = new List<FishModel>();
        public static event Action<Vector3> destinationSelectedEvent;

        private void FixedUpdate()
        {
            //TODO: FIX - Seems to be an offset when selecting fish
            if (InputSystem.GetDevice<Mouse>().press.wasPressedThisFrame)
            {
                RaycastHit hitinfo;
                hitinfo = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hitinfo))
                {
                    FishModel fish = hitinfo.transform.GetComponent<FishModel>();
                    if (fish != null)
                    {
                        if (activeFish.Count >= 1)
                        {
                            activeFish[0].isPlayerFish = false;
                            activeFish.Clear();
                        }

                        activeFish.Add(fish);
                        fish.isPlayerFish = true;
                    }
                    
                }
                else
                {
                    if (activeFish.Count >= 1)
                    {
                        Vector3 mousePos = new Vector3(Input.mousePosition.x, 0,  Input.mousePosition.z);
                        destinationSelectedEvent?.Invoke(mousePos);

                        Debug.Log("Destination: " + mousePos + " Z test: " + Input.mousePosition.z);
                    }
                }
            }

            //Returns a vector2
            //Mouse.current.position.ReadValue();
        }
    }
}
