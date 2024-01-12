using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    private InputAction click;

    public Transform aimTarget;
    public GameObject projectile;
    public Transform projectileSource;
    public float projectileVelocity = 30f;

    public float g = 9.81f;
    public float maxDist = 100f;
    // Start is called before the first frame update
    Attack currentAttack;
    public ElementDefinition element;
    InputAction fire;
    void Start()
    {
        currentAttack = new CurvedFire(element,new ExplosionCollisionBehaviour(element.explosionParticles));

        fire = GetComponent<PlayerInput>().actions.FindAction("Player/Attack");

        fire.started +=
            _ => OnStartAttack();

        fire.canceled +=
            _ => OnEndAttack();

    }

    void OnStartAttack(){
        currentAttack.OnStartAttack(aimTarget,projectileSource);
    }

    void OnDoAttack(){
        currentAttack.OnDoAttack(aimTarget,projectileSource);
    }

    void OnEndAttack(){
        currentAttack.OnEndAttack(aimTarget,projectileSource);
    }

    // Update is called once per frame
    void Update()
    {
        if(fire.enabled && fire.IsPressed()) OnDoAttack();
    }
}
