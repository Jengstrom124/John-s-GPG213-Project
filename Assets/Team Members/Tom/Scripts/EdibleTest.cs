using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdibleTest : MonoBehaviour, IEdible
{
    public void GetEaten(IPredator eatenBy)
    {
        Destroy(gameObject);
    }

    public EdibleInfo GetInfo()
    {
        return new EdibleInfo();
    }

    public void GotShatOut(IPredator shatOutBy)
    {
	    
    }
}
