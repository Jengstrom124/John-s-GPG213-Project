using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour
{
    List<FishModel> activeFish = new List<FishModel>();

    private void FixedUpdate()
    {
        //TODO: FIX - Seems to be an offset when selecting fish
        if(InputSystem.GetDevice<Mouse>().press.wasPressedThisFrame)
        {
            RaycastHit hitinfo;
            hitinfo = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitinfo))
            {
                FishModel fish = hitinfo.transform.GetComponent<FishModel>();
                if (fish != null)
                {
                    if(activeFish.Count >= 1)
                    {
                        activeFish[0].isPlayerFish = false;
                        activeFish.Clear();
                    }

                    activeFish.Add(fish);
                    fish.isPlayerFish = true;
                }
            }
        }

        //Returns a vector2
        //Mouse.current.position.ReadValue();
    }
}
