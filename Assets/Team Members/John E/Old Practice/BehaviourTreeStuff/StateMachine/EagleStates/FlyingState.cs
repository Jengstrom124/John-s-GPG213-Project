using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingState : StateBase
{
    public override void Enter()
    {
        //base.Enter();
        Debug.Log("Flying State");
    }

    public override void Execute()
    {
        //base.Execute();

    }

    public override void Exit()
    {
        //base.Exit();
    }
}
