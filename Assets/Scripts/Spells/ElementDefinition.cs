using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ElementDefinition", fileName ="Spell/ElementDefinition",order =1)]
public class ElementDefinition : ScriptableObject
{
    public int damage = 50;
    public ProjectileDefinition baseProjectile;
    public GameObject explosionParticles;

    public float standardVelocity;
}
