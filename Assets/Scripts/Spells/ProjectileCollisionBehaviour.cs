using UnityEngine;


public abstract class CollisionBehaviour
{
    public ElementDefinition element;
    public abstract void PerformCollision(GameObject projectile,GameObject model,GameObject effect, Collision other, Rigidbody rb);
}


public class ExplosionCollisionBehaviour:CollisionBehaviour
{
    const float DELETE_TIME = 10f; 
    GameObject _explosionParticles;


    public ExplosionCollisionBehaviour(GameObject explosionParticles){
        _explosionParticles = explosionParticles;
    }

    public override void PerformCollision(GameObject projectileObject,GameObject model,GameObject effect, Collision other, Rigidbody rb){
        if(_explosionParticles){
            GameObject explosionInstance = GameObject.Instantiate(_explosionParticles,projectileObject.transform.position,projectileObject.transform.rotation);
            GameObject.Destroy(explosionInstance,DELETE_TIME);
        }

        foreach(ParticleSystem particleSystem in projectileObject.GetComponentsInChildren<ParticleSystem>()){
            particleSystem.Stop();
        }
        
        model.SetActive(false);
        
        foreach(Collider col in projectileObject.GetComponents<Collider>()){
            col.enabled = false;
        }
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true; 
        GameObject.Destroy(projectileObject,DELETE_TIME);
    }
}