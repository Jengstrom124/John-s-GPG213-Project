using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public StateBase currentState;

    private void Start()
    {
        currentState.Enter();
    }
    // Update is called once per frame
    void Update()
    {
        currentState.Execute();
    }

    public void ChangeState(StateBase newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
