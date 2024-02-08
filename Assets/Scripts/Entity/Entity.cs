using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private IHealth _health;
    public UnityEvent onDeathEvent;

    void Start()
    {
        _health = GetComponent<IHealth>();
        if(_health != null){
            _health.RegisterOnDeathEvent(OnDeath);
        }
    }

    // Update is called once per frame
    public void ApplyDamage(int value){
        Debug.Log("Is Damaged");
        if(_health != null){
            _health.ApplyDamage(value);
        }
    }

    void OnDeath(){
        Debug.Log("Died");
        onDeathEvent?.Invoke();
        if(_health != null){
            _health.UnRegisterOnDeathEvent(OnDeath);
        }
    }
}

