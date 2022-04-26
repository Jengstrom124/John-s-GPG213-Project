using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace John
{
    public class JohnRTSTestController : ManagerBase<JohnRTSTestController>
    {
        List<IRTS> currentSelectedRTSEntity = new List<IRTS>();
        public static event Action<Vector3> destinationSelectedEvent;

        private void FixedUpdate()
        {
            //TODO: FIX - Seems to be an offset when selecting fish
            //TODO: Water may prevent raycast getting to fish
            if (InputSystem.GetDevice<Mouse>().press.wasPressedThisFrame)
            {
                //Raycast to mouse position
                RaycastHit hitinfo;
                hitinfo = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //Check what Raycast hit
                if (Physics.Raycast(ray, out hitinfo))
                {
                    //If we hit an entity
                    IRTS RTSEntity = hitinfo.transform.GetComponent<IRTS>();
                    if (RTSEntity != null)
                    {
                        //Upon selecting a new entity, reset the current one if there is one
                        if (currentSelectedRTSEntity.Count >= 1)
                        {
                            currentSelectedRTSEntity[0].Deselected();
                            currentSelectedRTSEntity.Clear();
                        }

                        //Update new entity
                        currentSelectedRTSEntity.Add(RTSEntity);
                        RTSEntity.Selected();
                    }
                    else
                    {
                        //If we hit water
                        if(hitinfo.transform.GetComponent<Water>() != null)
                        {
                            //Only if we are controlling an entity - set a destination
                            if (currentSelectedRTSEntity.Count >= 1)
                            {
                                Vector3 mousePos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);

                                //currentSelectedRTSEntity[0].SetDestination(mousePos);

                                Debug.Log("RTS INFO: Destination: " + mousePos + " Z test: " + Input.mousePosition.z);
                            }
                        }
                    }
                    
                }
            }

            //Returns a vector2
            //Mouse.current.position.ReadValue();
        }
    }
}
