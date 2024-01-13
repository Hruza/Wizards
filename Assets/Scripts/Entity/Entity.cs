using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
[System.Serializable]
public enum EntityType{
    inanimate = 0,
    player = 1,
    enemy = 2
}

public class Entity : MonoBehaviour
{
    public EntityType entityType = EntityType.player;
    public float maxHealth = 100f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public static class EntityTypeExtensions
{  
    public static bool HasFlag(this EntityType self, EntityType flag){
        return (self & flag) == flag;
    }

}

