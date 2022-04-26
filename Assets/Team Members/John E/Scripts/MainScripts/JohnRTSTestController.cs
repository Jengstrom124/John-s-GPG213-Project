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
        public event Action<GameObject> playerFishSelectedEvent;

        public GameObject activePlayerFish;

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

                            if(activePlayerFish != null)
                            {
                                activePlayerFish = null;
                                playerFishSelectedEvent?.Invoke(null);
                            }
                        }

                        //Update new entity
                        currentSelectedRTSEntity.Add(RTSEntity);
                        RTSEntity.Selected();

                        //If the entity is a fish- store this as a reference to influence aligning neighbouring fish
                        if(hitinfo.transform.GetComponent<FishBase>())
                        {
                            activePlayerFish = hitinfo.transform.gameObject;
                            playerFishSelectedEvent?.Invoke(activePlayerFish);
                        }
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

                                currentSelectedRTSEntity[0].SetDestination(mousePos);

                                Debug.Log("RTS INFO: Destination: " + mousePos);
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
