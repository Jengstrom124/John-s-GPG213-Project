using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OlliePlayerController : MonoBehaviour
{
    public KeyCode forward, backward, left, right;
    public GameObject currentlyPiloting;
    public OlliePlayerAvatar playerAvatar;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        playerAvatar.touchingCarEvent += ChangePiloting;
        OllieVehicleBase.exitVehicleEvent += ExitCar;
    }

    private void OnDisable()
    {
        playerAvatar.touchingCarEvent -= ChangePiloting;
        OllieVehicleBase.exitVehicleEvent -= ExitCar;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(forward))
        {
            currentlyPiloting.GetComponent<OllieVehicleBase>()?.Forward();
            currentlyPiloting.GetComponent<OlliePlayerAvatar>()?.Forward();
        }
        if (Input.GetKey(backward))
        {
            //currentlyPiloting.GetComponent<OllieVehicleBase>()?.Backward();
            currentlyPiloting.GetComponent<OlliePlayerAvatar>()?.Backward();
        }
        
        if ((Input.GetKey(left))&&((Input.GetKey(forward)||(Input.GetKey(backward)))))
        {
            currentlyPiloting.GetComponent<OllieVehicleBase>()?.Left();
            currentlyPiloting.GetComponent<OlliePlayerAvatar>()?.Left();
        }
        
        if ((Input.GetKey(right))&&((Input.GetKey(forward)||(Input.GetKey(backward)))))
        {
            currentlyPiloting.GetComponent<OllieVehicleBase>()?.Right();
            currentlyPiloting.GetComponent<OlliePlayerAvatar>()?.Right();
        }
        
        if (Input.GetKey(left)==false&&(Input.GetKey(right)==false))
        {
            
        }
    }

    void ChangePiloting(GameObject target)
    {
        print("press TAB to pilot " + target);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //currentlyPiloting.GetComponent<Renderer>().enabled = false;
            currentlyPiloting.SetActive(false);
            print(currentlyPiloting);
            currentlyPiloting = null;
            currentlyPiloting = target;
        }
    }

    
    //maybe take this out of event, and just put keycodeQ in update so you get out of whatever you're piloting
    void ExitCar()
    {
        print("exiting vehicle");
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAvatar.gameObject.SetActive(true);
            var pos = currentlyPiloting.gameObject.transform.localPosition;
            playerAvatar.transform.position = new Vector3(pos.x-1,pos.y,pos.z);
            currentlyPiloting = null;
            currentlyPiloting = playerAvatar.gameObject;
        }
    }
}
