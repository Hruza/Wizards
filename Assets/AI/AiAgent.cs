using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

 
public class AggroData{
    public Entity entity;
    public float aggro;
}

public enum OffMeshLinkMoveMethod {
   Teleport,
   NormalSpeed,
   Parabola,
   Curve
}
[RequireComponent (typeof (NavMeshAgent))]
public class AiAgent : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator anim;


    public OffMeshLinkMoveMethod method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve curve = new AnimationCurve ();

    [Header("Aggro")]
    public float attackThreshold = 100f;
    public float deaggroThreshold = 50f;
    public float LOSIncrement = 0.5f;
    public float detectionRadius = 30f;
    public float proximityIncrement = 10f;
    public float proximityRadius = 3f;
    public float attackIncrement = 150f;
    public float agroDecay = 0.5f;
    public float aggroCap = 200f;
    public LayerMask aggroLayerMask;
    public LayerMask obstacleLayerMask;

    public EntityType attackedTypes;

    private List<AggroData> aggroData;

    public Entity currentTarget;

    IEnumerator Start () {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.autoTraverseOffMeshLink = false;
        while (true) {
            if (nav.isOnOffMeshLink) {
                anim.SetBool("Jump",true);
                anim.SetBool("Grounded",false);
                if (method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine (NormalSpeed (nav));
                else if (method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine (Parabola (nav, 2.0f, 1f));
                else if (method == OffMeshLinkMoveMethod.Curve)
                    yield return StartCoroutine (Curve (nav, 1f));
                nav.CompleteOffMeshLink ();
            }
            else{
                anim.SetBool("Jump",false);
                anim.SetBool("Grounded",true);
            }
            yield return null;
        }
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

    public Vector3 currentTargetPosition{
        get{
            return nav.destination;
        }
    }

    void UpdateAggro(Entity entity,float valueIncrement){
        AggroData result = aggroData.Find(x => x.entity == entity);
        if(result != null){
            result.aggro = Mathf.Clamp(result.aggro + valueIncrement,0f,aggroCap);
        }
        else if(valueIncrement>0){
            aggroData.Add(new AggroData{
                entity=entity,
                aggro =valueIncrement
            });
        }
    }

    void UpdateAllAggro(float valueIncrement){
        aggroData.ForEach(x => x.aggro += valueIncrement);
        aggroData.RemoveAll(x => x.aggro <0);
    }

    void EvaluateAggro(){
        foreach(Collider coll in Physics.OverlapSphere(transform.position,detectionRadius,aggroLayerMask.GetHashCode())){
            Entity entity = coll.GetComponent<Entity>();

            if(entity == null) continue;
            
            if(!attackedTypes.HasFlag(entity.entityType)) continue; 

            GameObject go = coll.gameObject;

            if(go == gameObject) continue;

            Vector3 direction = go.transform.position - transform.position;

            float aggroChange = 0;

            if(direction.sqrMagnitude < proximityRadius*proximityRadius){
                aggroChange += proximityIncrement;
            }

            //RaycastHit hit = Physics.Raycast(transform.position,direction,detectionRadius,obstacleLayerMask);
        }
    
    }

    void Update(){  
        anim.SetFloat("Speed",nav.velocity.magnitude);
    }

    void OnFootstep(){

    }

    void OnLand(){
        
    }

    IEnumerator NormalSpeed (NavMeshAgent agent) {
      OffMeshLinkData data = agent.currentOffMeshLinkData;
      Vector3 endPos = data.endPos + Vector3.up*agent.baseOffset;
      while (agent.transform.position != endPos) {
        agent.transform.position = Vector3.MoveTowards (agent.transform.position, endPos, agent.speed*Time.deltaTime);
        yield return null;
      }
    }
    IEnumerator Parabola (NavMeshAgent agent, float height, float duration) {
      OffMeshLinkData data = agent.currentOffMeshLinkData;
      Vector3 startPos = agent.transform.position;
      Vector3 endPos = data.endPos + Vector3.up*agent.baseOffset;
      float normalizedTime = 0.0f;
      while (normalizedTime < 1.0f) {
        float yOffset = height * 4.0f*(normalizedTime - normalizedTime*normalizedTime);
        agent.transform.position = Vector3.Lerp (startPos, endPos, normalizedTime) + yOffset * Vector3.up;
        normalizedTime += Time.deltaTime / duration;
        yield return null;
      }
    }
    IEnumerator Curve (NavMeshAgent agent, float duration) {
      OffMeshLinkData data = agent.currentOffMeshLinkData;
      Vector3 startPos = agent.transform.position;
      Vector3 endPos = data.endPos + Vector3.up*agent.baseOffset;
      float normalizedTime = 0.0f;
      while (normalizedTime < 1.0f) {
        float yOffset = curve.Evaluate (normalizedTime);
        agent.transform.position = Vector3.Lerp (startPos, endPos, normalizedTime) + yOffset * Vector3.up;
        normalizedTime += Time.deltaTime / duration;
        yield return null;
      }
    }
}

 