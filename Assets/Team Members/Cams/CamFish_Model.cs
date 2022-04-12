using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFish_Model : MonoBehaviour, IEdible
{
	public float amount;

    public void GetEaten(IPredator eatenBy)
    {
	    eatenBy.GotFood(amount);

	    Destroy(gameObject);
    }
}
