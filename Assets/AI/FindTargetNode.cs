using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTargetNode : ActionNode
{

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
    
    }

    protected override State OnUpdate()
    {
        GameObject target = GameObject.FindWithTag("Player");
        if(target != null){
            blackboard.moveToObject = target;
            blackboard.moveToPosition = target.transform.position;
            return State.Success;
        }
        else{
            return State.Failure;
        }
    }
}
