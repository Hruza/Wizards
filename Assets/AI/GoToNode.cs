using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNode : ActionNode
{

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
    
    }

    protected override State OnUpdate()
    {
        agent.GoToTarget(blackboard.moveToPosition);
        return State.Success;
    }
}
