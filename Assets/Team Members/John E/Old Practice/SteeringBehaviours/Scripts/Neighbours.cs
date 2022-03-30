using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbours : MonoBehaviour
{
    public List<GameObject> neighbours;

    public List<Collider> fishColliders = new List<Collider>();


    private void OnTriggerEnter(Collider other)
    {
        if(other == other.GetComponent<CapsuleCollider>())
        {
            Debug.Log("Skip");
        }
        else
        {
            if (other.GetComponent<BoidModel>() != null && !neighbours.Contains(other.gameObject))
            {
                neighbours.Add(other.gameObject);
            }
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
