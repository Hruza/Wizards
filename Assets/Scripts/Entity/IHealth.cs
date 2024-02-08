
using System;

public interface IHealth{
    public int GetCurrentHealth();
    public int ApplyDamage(int damage);
    public void RegisterOnDeathEvent(Action action);
    public void UnRegisterOnDeathEvent(Action action);
}