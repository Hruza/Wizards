using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToTargetNode : ActionNode
{
    public float distance = 2f;
    public float timeAfterLastKnownPosition=4f;
    public bool stopInReach = true;
    public bool demandVisibility = true;
    private Vector3 latestKnownPosition;
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {
    
    }

    protected override State OnUpdate()
    {
        AggroData target = agent.currentTarget;
        if(target != null){
            blackboard.target = target;
        }
        else{
            blackboard.target = null;
            return State.Failure;
        }

        if((target.entity.transform.position - agent.transform.position).sqrMagnitude < distance*distance){
            if(!demandVisibility || agent.CheckVisibility(target.entity,distance)){
                if(stopInReach){
                    agent.Stop();
                }
                return State.Success;
            }
        }


        if(Time.time - target.lastPositionUpdate < timeAfterLastKnownPosition){
            latestKnownPosition = target.entity.transform.position;
        }
        else if(latestKnownPosition == Vector3.zero){
            latestKnownPosition = target.lastKnownPosition;
        }
        agent.GoToTarget(latestKnownPosition);
        return State.Running;
    }
}
