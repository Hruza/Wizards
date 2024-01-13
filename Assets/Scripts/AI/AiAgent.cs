using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

 
public class AggroData{
    public Entity entity;
    public float aggro;
    public Vector3 lastKnownPosition;
    public float lastPositionUpdate;
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

    const float AGGRO_CALC_PERIOD = 0.25f;

    private NavMeshAgent nav;
    private Animator anim;


    public OffMeshLinkMoveMethod method = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve curve = new AnimationCurve ();

    [Header("Aggro")]

    public Transform eyes;
    public TargetDetectionSettings aggroSettings;
    private float detectionCos;

    private List<AggroData> aggroData;

    public AggroData _currentTarget;
    public AggroData currentTarget{
        get{
            if(_currentTarget == null || _currentTarget.entity == null) return null;
            return _currentTarget;
        }
        private set{
            if(value == null || value.entity == null) _currentTarget = null;
            _currentTarget = value;
        }
    }

    IEnumerator Start () {
        detectionCos = Mathf.Cos(aggroSettings.detectionAngle/2 * Mathf.Deg2Rad);

        aggroData = new List<AggroData>();

        if(eyes == null) eyes = transform;

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.autoTraverseOffMeshLink = false;

        StartCoroutine(EvaluateAggro());

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

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        if(eyes) Gizmos.DrawWireSphere(eyes.position,aggroSettings.detectionRadius);
        else Gizmos.DrawWireSphere(transform.position,aggroSettings.detectionRadius);
    }

    public void GoToTarget(Vector3 target){
        if((nav.destination - target).sqrMagnitude > 0.5f){
            nav.destination = target;
        }
    }

    public void Stop(float distance = 0.1f){
        nav.destination = transform.position + distance * nav.velocity;
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

    void UpdateAggro(Entity entity,float valueIncrement,bool updatePosition, bool isAttack=false){
        AggroData result = aggroData.Find(x => x.entity == entity);
        if(result != null){
            result.aggro += Mathf.Max(Mathf.Min(result.aggro + valueIncrement,isAttack?aggroSettings.attackAggroCap:aggroSettings.aggroCap) - result.aggro, 0);
            result.lastKnownPosition = entity.transform.position;
            result.lastPositionUpdate = Time.time;
        }
        else if(valueIncrement>0){
            aggroData.Add(new AggroData{
                entity=entity,
                aggro =valueIncrement,
                lastKnownPosition = entity.transform.position,
                lastPositionUpdate = Time.time,
            });
        }
    }

    void UpdateAllAggro(float valueIncrement){
        aggroData.ForEach(x => x.aggro += valueIncrement);
        aggroData.RemoveAll(x => x.aggro <0 || x.entity == null); 
    }

    AggroData GetMaxAggroEnemy(){
        if(aggroData.Count == 0){
            return null;
        }
        AggroData maxAggro =  aggroData.Find(x => x.aggro == aggroData.Max(y => y.aggro));
        if(maxAggro.aggro > aggroSettings.attackThreshold) return maxAggro;
        else return null;
    }

    public bool CheckVisibility(Entity entity,float maxDist){
        Vector3 direction = entity.gameObject.transform.position - eyes.position;
        float distance = direction.magnitude;
        RaycastHit hit; 
        LayerMask raycastMask = aggroSettings.obstacleLayerMask | (1 << entity.gameObject.layer);

        bool isVisible = false;

        //check angle
        if(distance > 0 ? Vector3.Dot(direction,eyes.forward)/distance < detectionCos : false){
            return false;
        } 
    

        for (float i = 0; i < 2; i+=0.2f){
            if(Physics.Raycast(eyes.position, direction + i * Vector3.up, out hit,maxDist, raycastMask)){
                if(hit.collider.GetComponent<Entity>() != entity){ 
                    Debug.DrawLine(eyes.position,hit.point,Color.red,AGGRO_CALC_PERIOD);
                }
                else{
                    isVisible=true;
                    break;
                }
            } 
        }
        return isVisible;
    }

    IEnumerator EvaluateAggro(){
        float aggroChange;
        float distance;
        bool currentProcessed;

        while(true){
            currentProcessed = false;
            foreach(Collider coll in Physics.OverlapSphere(eyes.position,aggroSettings.detectionRadius,aggroSettings.aggroLayerMask.value)){
                Entity entity = coll.GetComponent<Entity>();

                if(entity == null) continue;
                
                if(!aggroSettings.attackedTypes.HasFlag(entity.entityType)) continue; 

                GameObject go = coll.gameObject;

                if(currentTarget != null && entity == currentTarget.entity) currentProcessed = true;

                if(go == gameObject) continue;

                Vector3 direction = go.transform.position - eyes.position;

                aggroChange = 0;

                distance = direction.magnitude;

                //add proximity aggro
                if(distance < aggroSettings.proximityRadius){
                    aggroChange += aggroSettings.proximityIncrement;
                    Debug.DrawRay(eyes.position,direction+Vector3.up,Color.green,AGGRO_CALC_PERIOD); 
                }

                bool isVisible = true;
                            
                //check obstacles
                //add look aggro
                if(distance < aggroSettings.detectionRadius){
                    isVisible = CheckVisibility(entity,aggroSettings.detectionRadius);
                    if(isVisible){
                        aggroChange += aggroSettings.LOSIncrement * Mathf.Lerp(0.5f ,1f ,distance/aggroSettings.detectionRadius);
                        
                        Debug.DrawRay(eyes.position,direction,Color.yellow,AGGRO_CALC_PERIOD);
                    }
                    else{
                        Debug.DrawRay(eyes.position,direction,Color.blue,AGGRO_CALC_PERIOD);
                    }
                }
                
                if(aggroChange != 0){
                    UpdateAggro(entity,aggroChange,isVisible);
                }
            }

            if(!currentProcessed && currentTarget != null && CheckVisibility(currentTarget.entity, aggroSettings.visionRadius)) {
                UpdateAggro(currentTarget.entity,0,true);
            }

            UpdateAllAggro(-aggroSettings.aggroDecay);

            UpdateCurrentTarget();

            yield return new WaitForSeconds(AGGRO_CALC_PERIOD);
        }
    }
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.gray;
        for (int i = 0; i < aggroData.Count; i++)
        {
            if(aggroData[i] == null || aggroData[i].entity == null || aggroData[i].entity.gameObject == null) continue;
            GUI.Label(new Rect(10, 10*i, 100, 20), aggroData[i].entity.gameObject.name + ": " +aggroData[i].aggro.ToString(),new GUIStyle());
        }
    }
    void UpdateCurrentTarget(){
        
        AggroData maxAggroEnemy = GetMaxAggroEnemy();
        if(currentTarget == null || currentTarget.entity == null || currentTarget.aggro <= 0){
            if(maxAggroEnemy != null && maxAggroEnemy.aggro > aggroSettings.attackThreshold){
                currentTarget = maxAggroEnemy;
            }
            else{
                currentTarget = null;
            }
        }
        else if(maxAggroEnemy != null && maxAggroEnemy.aggro - currentTarget.aggro > aggroSettings.targetSwitchThreshold){
            currentTarget = maxAggroEnemy;
        }
    }

    void Update(){  
        anim.SetFloat("Speed",nav.velocity.magnitude);
    }

    void OnFootstep(){

    }

    void OnLand(){
        
    }

    private void OnDisable() {
        nav.enabled = false;
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

[Serializable]
public class TargetDetectionSettings{
    public float attackThreshold = 100f;
    public float deaggroThreshold = 50f;
    public float targetSwitchThreshold = 20f;
    public float LOSIncrement = 5f;
    public float detectionRadius = 30f;
    public float visionRadius = 70f;
    public float detectionAngle = 180f;
    public float proximityIncrement = 10f;
    public float proximityRadius = 3f;
    public float attackIncrement = 150f;
    public float aggroDecay = 2f;
    public float aggroCap = 200f;
    public float attackAggroCap = 300f;
    [Tooltip("Layers to check for possible targets.")]
    public LayerMask aggroLayerMask;
    [Tooltip("Layers to break line of sight.")]
    public LayerMask obstacleLayerMask;
    [Tooltip("Type of entities that can be targeted.")]
    public EntityType attackedTypes;

}