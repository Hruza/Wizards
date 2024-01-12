using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Attack
{
    const float G = 9.81f;   
    protected ElementDefinition _element; 
    protected CollisionBehaviour _collisionBehaviour;

    public Attack(ElementDefinition element, CollisionBehaviour collisionBehaviour){
        _element = element; 
        _collisionBehaviour = collisionBehaviour;
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

    protected (GameObject,Projectile) CreateProjectile(GameObject projectile,Vector3 pos, Quaternion rot,float scale){
        GameObject proj = GameObject.Instantiate(projectile,pos,rot);

        Projectile projComponent = proj.GetComponent<Projectile>();
        if(projComponent){
            projComponent.collBehaviour = _collisionBehaviour;
            projComponent.scale = scale;
        }
        return (proj,projComponent);
    }
}

public class DirectFire:Attack
{
    public DirectFire(ElementDefinition element, CollisionBehaviour collisionBehaviour) : base(element, collisionBehaviour)
    {
    }

    public override void OnStartAttack(Transform target,Transform source)
    {
        GameObject proj;
        Projectile projComponent;
        (proj, projComponent) = CreateProjectile(_element.baseProjectile,source.position,source.rotation,1);

        projComponent.g = 0;

        proj.GetComponent<Rigidbody>().linearVelocity = (target.position - source.position).normalized * _element.standardVelocity;

    }
}

public class CurvedFire:Attack
{
    public CurvedFire(ElementDefinition element, CollisionBehaviour collisionBehaviour) : base(element, collisionBehaviour)
    {
    }

    public override void OnStartAttack(Transform target,Transform source)
    {
        GameObject proj;
        Projectile projComponent;
        (proj, projComponent) = CreateProjectile(_element.baseProjectile,source.position,source.rotation,1);

        proj.GetComponent<Rigidbody>().linearVelocity = GetParabolicVelocity(source.position, target.position, _element.standardVelocity, projComponent.g , 50f);

    }
}