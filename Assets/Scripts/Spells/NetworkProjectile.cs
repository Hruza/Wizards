using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

public interface IProjectile: ITickable{
    public void Initialize(ExplosionCollisionBehaviour collBehaviour, float scale, float g, float initSpeed); 
    public void HideProjectile();
    public void StopProjectile();
    public void ExplosionEffect(Vector3 position, Quaternion rotation);
}

public class NetworkProjectile : NetworkBehaviour, IProjectile
{
    const float DESPAWN_TIME = 50f;
    Rigidbody rb;
    public GameObject explosionEffect;
    public GameObject effect;
    public GameObject model;
    public float g = 9.81f;
    public CollisionBehaviour collBehaviour;
    public float scale = 1;
    public float initSpeed = 0;
    float despawnTime;
    bool initialized;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        Debug.Log("Projectile onSpawned called");
        if(!IsServer) {
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            return;
        }
    }

    public void ProcessTick(){}

    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    public void Initialize(ExplosionCollisionBehaviour collBehaviour, float scale, float g, float initSpeed) {
        this.collBehaviour = collBehaviour;
        this.scale = scale;
        this.g = g;
        this.initSpeed = initSpeed;


        Debug.Log("Projectile enabled");
        despawnTime = Time.time + DESPAWN_TIME;
        rb.isKinematic = false; 
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        GetComponent<Collider>().enabled = true;
        if(initSpeed>0) rb.linearVelocity = transform.forward * initSpeed;
        if(scale != 1){
            model.transform.localScale *= scale;
            effect.transform.localScale *= scale;
        }
        initSpeed = 0;
        ShowProjectile();
        initialized = true;
    }

    void OnDisable(){
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true; 
        initialized = false;
    }

    // Update is called once per frame  
    void Update()  
    {
        if(!IsServer || !initialized) return;
        if(rb && !rb.isKinematic){
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity,Vector3.up);
        }
        if(Time.time > despawnTime){
            OnDisable();
            GetComponent<NetworkObject>().Despawn();
        }
    }

    private void FixedUpdate() {
        if(!IsServer) return;
        if(rb && !rb.isKinematic){
            rb.AddForce(Vector3.down * g * rb.mass);
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(!IsServer) return;
        if(rb != null){
            collBehaviour.PerformCollision(gameObject,this,other.collider);
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
        HideProjectileClientRPC();
        
    }

    public void StopProjectile(){
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true; 
    }

    public void ExplosionEffect(Vector3 position, Quaternion rotation){
        ExplosionEffectClientRPC(position, rotation);
    }

    [ClientRpc]
    public void HideProjectileClientRPC(){
        foreach(ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>()){
            particleSystem.Stop();
        }
        
        model.SetActive(false);
    }

    [ClientRpc]
    public void ExplosionEffectClientRPC(Vector3 position,Quaternion rotation){
        if(explosionEffect){
            GameObject explosionInstance = Instantiate(explosionEffect,position,rotation);
            Destroy(explosionInstance,5);
        }
    }

    public void DespawnObject(float t){
        if(Time.time + t < despawnTime) despawnTime = Time.time + t;
    }
}