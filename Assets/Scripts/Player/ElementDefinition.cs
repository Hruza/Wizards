using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ElementDefinition", fileName ="Custom/ElementDefinition",order =10)]
public class ElementDefinition : ScriptableObject
{
    public GameObject baseProjectile;
    public GameObject explosionParticles;

    public float standardVelocity;
}
