using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdibleTest : MonoBehaviour, IEdible
{
    public void GetEaten(EatAbility eatenBy)
    {
        Destroy(gameObject);
    }
}
