using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTargetNode : ActionNode
{
    public bool inLOS = true;
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
    
    }

    protected override State OnUpdate()
    {
        
        AggroData target = agent.currentTarget;
        Debug.Log(target);
        if(target != null){
            blackboard.target = target;
            return State.Success;
        }
        else{
            blackboard.target = null;
            return State.Failure;
        }
    }
}
