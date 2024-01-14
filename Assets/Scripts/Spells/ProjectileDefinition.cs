using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileDefinition", menuName = "Spell/ProjectileDefinition", order = 4)]
public class ProjectileDefinition : ScriptableObject {
    public GameObject prefab;
    public float gravity = 9.81f/2;

}