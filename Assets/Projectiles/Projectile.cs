using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    const float DELETE_TIME = 5f;
    Rigidbody rb;
    public GameObject explosion;
    public GameObject effect;
    public GameObject model;
    public float g;
    public CollisionBehaviour collBehaviour;
    public float scale = 1;
    private Vector3 _scaleVector;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(scale != 1){
            model.transform.localScale *= scale;
            effect.transform.localScale *= scale;
        }
    }

    // Update is called once per frame  
    void Update()  
    {
        if(rb && !rb.isKinematic){
            transform.rotation = Quaternion.LookRotation(rb.velocity,Vector3.up);
        }
    }

    private void FixedUpdate() {
        rb.AddForce(Vector3.down * g * rb.mass);
    }

    private void OnCollisionEnter(Collision other) {
        
        collBehaviour.PerformCollision(gameObject,model,effect,other,rb);

    }
}