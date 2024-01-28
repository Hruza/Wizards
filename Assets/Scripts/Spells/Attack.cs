using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Attack:ScriptableObject
{ 
    protected Spell _spellReference;
    [SerializeField]AttackParameters defaultParameters;
    protected AttackParameters parameters;
    public void Initialise(Spell spellReference) {
        _spellReference = spellReference;
        parameters = defaultParameters.Combine(_spellReference.modifier.parameters);
    }

    public virtual void OnStartAttack(Transform target,Transform source)
    {

    }
    public virtual void OnDoAttack(Transform target,Transform source)
    {

    }
    public virtual void OnEndAttack(Transform target,Transform source)
    {

    }

    protected Vector3 GetParabolicVelocity(Vector3 start, Vector3 target, float speed, float g, float maxDist){
        Vector3 dir = target - start;
        float mag = dir.magnitude;  
        if(mag>maxDist){
            dir *= maxDist/mag;
            mag = maxDist;
        }
        float y = dir.y;
        Vector2 horizontal = new Vector2(dir.x,dir.z);
        float x = horizontal.magnitude;
        horizontal = horizontal/x;

        float t = mag/speed;

        y = (y + (g* t*t/2))/t;
        x = x/t;

        return new Vector3(horizontal.x * x, y, horizontal.y * x);
    }

    protected void CreateProjectile(GameObject projectile,Vector3 pos, Vector3 velocity,float scale, float g){
        _spellReference.manager.CreateProjectile(projectile,pos,velocity,scale,g);
    }

}
