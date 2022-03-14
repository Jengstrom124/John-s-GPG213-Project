using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Obstacle : MonoBehaviour
{
    [Header("Ref Only")]
    Vector3 currentPos;
    public Vector3 previousPos;
    bool alreadyUpdatingGrid = false;

    Node centreNode;
    public event Action<GameObject> OnMovedEvent;


    private void Start()
    {
        currentPos = transform.position;
    }

    private void Update()
    {
        if(transform.position != currentPos && !alreadyUpdatingGrid)
        {
            alreadyUpdatingGrid = true;
            StartCoroutine(UpdateGrid());
        }
    }

    IEnumerator UpdateGrid()
    {
        yield return new WaitForSeconds(2f);

        previousPos = currentPos;
        currentPos = transform.position;
        OnMovedEvent?.Invoke(gameObject);
        alreadyUpdatingGrid = false;
    }
}
