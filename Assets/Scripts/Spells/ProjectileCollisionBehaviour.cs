using UnityEngine;
using Unity.Netcode;
using System;


public abstract class CollisionBehaviour
{
    public ElementDefinition element;
    public abstract void PerformCollision(GameObject projectileObject,Projectile projComponent, Collision other, Rigidbody rb);
}


public class ExplosionCollisionBehaviour:CollisionBehaviour
{
    const float DELETE_TIME = 4f; 

    public override void PerformCollision(GameObject projectileObject,Projectile projComponent, Collision other, Rigidbody rb){

        projComponent.HideProjectileClientRPC();
        
        projComponent.ExplosionEffectClientRPC(projectileObject.transform.position, Quaternion.identity);
        
        foreach(Collider col in projectileObject.GetComponents<Collider>()){
            col.enabled = false;
        }
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true; 
        projComponent.DespawnObject(DELETE_TIME);
    }
}