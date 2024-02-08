using UnityEngine;
using Unity.Netcode;
using System;

public class NetworkHealth : NetworkBehaviour, IHealth
{
    private NetworkVariable<int> _health = new NetworkVariable<int>();
    public int maxHealth = 100;

    private Action onDamage;
    private Action onDeath;

    public override void OnNetworkSpawn()
    {
        if(IsServer){
            _health.Value = maxHealth;
        }
        _health.OnValueChanged += NotifyDamage;
    }

    public override void OnNetworkDespawn()
    {
        _health.OnValueChanged -= NotifyDamage;
    }

    private void NotifyDamage(int previous,int current){
        if(previous > 0){
            onDamage?.Invoke();
            if(current <= 0){
                NotifyDeath();
            }
        }
    }

    
    private void NotifyDeath(){
        onDeath?.Invoke();
        _health.OnValueChanged -= NotifyDamage;
    }

    public int ApplyDamage(int damage)
    {
        return _health.Value = Mathf.Max(_health.Value - damage,0);
    }

    public int GetCurrentHealth()
    {
        return _health.Value;
    }

    public void RegisterOnDeathEvent(Action action)
    {
        onDeath += action;
    }

    public void UnRegisterOnDeathEvent(Action action)
    {
        onDeath -= action;
    }
}
