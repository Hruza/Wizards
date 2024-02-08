using UnityEngine;
using Unity.Netcode;
using System;


public abstract class CollisionBehaviour
{
    public ElementDefinition element;
    public abstract void PerformCollision(GameObject projectileObject,IProjectile projComponent, Collider other);
}


public class ExplosionCollisionBehaviour:CollisionBehaviour
{
    const float DELETE_TIME = 4f; 

    public override void PerformCollision(GameObject projectileObject,IProjectile projComponent, Collider other){
        projComponent.HideProjectile();
        
        projComponent.ExplosionEffect(projectileObject.transform.position, Quaternion.identity);
        
        foreach(Collider col in projectileObject.GetComponents<Collider>()){
            col.enabled = false;
        }
        other.GetComponent<Entity>()?.ApplyDamage(projComponent.GetDamage());
        GameObject.Destroy(projectileObject,DELETE_TIME);
    }
}