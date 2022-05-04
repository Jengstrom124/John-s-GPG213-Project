using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTestScript : MonoBehaviour, IEdible
{
    public void GetEaten(IPredator eatenBy)
    {
        throw new System.NotImplementedException();
    }

    public EdibleInfo GetInfo()
    {
        throw new System.NotImplementedException();
    }

    public void GotShatOut(IPredator shatOutBy)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
