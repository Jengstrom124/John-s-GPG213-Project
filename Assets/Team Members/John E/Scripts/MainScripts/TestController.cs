using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace John
{
    public class TestController : MonoBehaviour
    {
        List<FishModel> activeFish = new List<FishModel>();
        public static event Action<Vector2> destinationSelectedEvent;

        //Camera Test
        public Tom.CameraFollow cam;
        Transform playerFish;
        float offsetValue;

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
                        if(cam != null)
                            cam.target = fish.transform;

                        playerFish = fish.transform;
                    }
                    
                }
                else
                {
                    if (activeFish.Count >= 1)
                    {
                        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.z);
                        destinationSelectedEvent?.Invoke(mousePos);

                        Debug.Log("Destination: " + mousePos);
                    }
                }
            }

            //Returns a vector2
            //Mouse.current.position.ReadValue();
        }

        private void Update()
        {
            if(cam != null)
            {
                offsetValue = cam.offset.y + (InputSystem.GetDevice<Mouse>().scroll.ReadValue().normalized.y * -InputSystem.GetDevice<Mouse>().scroll.ReadValue().normalized.y);

                if (cam.target == playerFish)
                    cam.offset = new Vector3(0, Mathf.Clamp(offsetValue, 30, 100), 0);
            }
        }
    }
}
