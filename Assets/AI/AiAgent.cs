using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    private NavMeshAgent nav;

    void Start(){
        nav = GetComponent<NavMeshAgent>();
    }

    public void GoToTarget(Vector3 target){
        if(nav.destination != target){
            nav.destination = target;
        }
    }

    public bool hasTarget{
        get{
           if(nav == null || !nav.hasPath) {
                return false;
            } 
            else{
                return true;
            }
        }
    }

    public Vector3 currentTarget{
        get{
            return nav.destination;
        }
    }

}
