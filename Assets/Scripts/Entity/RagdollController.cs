using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public GameObject skeleton;
    private Rigidbody[] _ragdollRigidbodies;
    private Collider[] _ragdollColliders;

    private Animator _anim;
    // Start is called before the first frame update

    void Start()
    {
        _anim = GetComponent<Animator>();
        _ragdollRigidbodies = skeleton.GetComponentsInChildren<Rigidbody>();
        _ragdollColliders = skeleton.GetComponentsInChildren<Collider>();
        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        foreach(Rigidbody rb in _ragdollRigidbodies){
            rb.isKinematic = false;
        }
        foreach(Collider coll in _ragdollColliders){
            coll.enabled = true;
        }
        if(_anim != null){
            _anim.enabled = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach(Rigidbody rb in _ragdollRigidbodies){
            rb.isKinematic = true;
        }
        foreach(Collider coll in _ragdollColliders){
            coll.enabled = false;
        }
        if(_anim != null){
            _anim.enabled = true;
        }
    }
}
