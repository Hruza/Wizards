using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(SphereCollider))]
public class Projectile : MonoBehaviour, IProjectile
{
    const float DESPAWN_TIME = 50f;
    public GameObject explosionEffect;
    public GameObject effect;
    public GameObject model;
    [HideInInspector]public float g = 9.81f;
    public CollisionBehaviour collBehaviour;

    public LayerMask layerMask;
    public float scale = 1;
    public float initSpeed = 0;
    float despawnTime;
    bool active;
    float detectionRadius;

    Vector3 velocity;


    public void Initialize(ExplosionCollisionBehaviour collBehaviour, float scale, float g, float initSpeed) {
        this.collBehaviour = collBehaviour;
        this.scale = scale;
        this.g = g;
        this.initSpeed = initSpeed;

        velocity = transform.forward * initSpeed;

        detectionRadius = GetComponent<SphereCollider>().radius;
        GetComponent<Collider>().enabled = false;

        Debug.Log("Projectile enabled");
        despawnTime = Time.time + DESPAWN_TIME;
        GetComponent<Collider>().enabled = true;
        if(scale != 1){
            model.transform.localScale *= scale;
            effect.transform.localScale *= scale;
        }
        initSpeed = 0;

        ShowProjectile();

        active = true;
    }

    private void OnEnable() {
        NetworkTickClock.instance.processTick += ProcessTick;
    }

    void OnDisable(){
        NetworkTickClock.instance.processTick -= ProcessTick;
        active = false;
    }
 
    void Update()  
    {
        transform.rotation = Quaternion.LookRotation(velocity,Vector3.up);
        if(Time.time > despawnTime){
            OnDisable();
            Destroy(gameObject);
        }
    }

    public void ShowProjectile(){
        effect.SetActive(true);
        model.SetActive(true);
        foreach(ParticleSystem particleSystem in effect.GetComponentsInChildren<ParticleSystem>()){
            particleSystem.Play();
        }
    }

    public void HideProjectile(){
        foreach(ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>()){
            particleSystem.Stop();
        }
        model.SetActive(false);
        active = false;
    }

    public void StopProjectile(){
        velocity = Vector3.zero;
    }

    RaycastHit hit;

    public void ProcessTick() {
        if(!active) return;
        Debug.DrawLine( - velocity.normalized*detectionRadius + transform.position,transform.position + (velocity*Time.fixedDeltaTime) + velocity.normalized*detectionRadius,Random.ColorHSV(),1f);
        if(velocity.sqrMagnitude >0 && Physics.CapsuleCast(transform.position,transform.position + (velocity*Time.fixedDeltaTime),detectionRadius,transform.forward,out hit,velocity.magnitude*Time.fixedDeltaTime,layerMask)){
            Debug.Log(hit.collider.gameObject);
            Debug.DrawLine(transform.position,hit.point,Color.red,10f);
            collBehaviour.PerformCollision(gameObject,this,hit.collider);
        }
        else{
            transform.position += velocity*Time.fixedDeltaTime;
        }
        velocity += g*Vector3.down*Time.fixedDeltaTime;
    }

    public void ExplosionEffect(Vector3 position, Quaternion rotation){
        if(explosionEffect){
            GameObject explosionInstance = Instantiate(explosionEffect,position,rotation);
            Destroy(explosionInstance,5);
        }
    }

}