using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbours : MonoBehaviour
{
    public List<GameObject> neighbours;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<MoveForwards>() != null && !neighbours.Contains(other.gameObject))
        {
            neighbours.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (neighbours.Contains(other.gameObject))
        {
            neighbours.Remove(other.gameObject);
        }
    }
}
