using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SpellModifier", menuName = "Spell/SpellModifier", order = 3)]
public class SpellModifier : ScriptableObject {
    public AttackParameters parameters;
}

[System.Serializable]
public class AttackParameters{
    public float scale = 1;
    public float speed = 1;
    public float damage = 1;
    public AttackParameters Combine(AttackParameters other){
        return new AttackParameters(){
            scale = scale*other.scale,
            speed = speed*other.speed,
            damage = damage*other.damage
        };
    }

    public AttackParameters(){

    }
}