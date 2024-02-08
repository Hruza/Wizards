using UnityEngine;

[CreateAssetMenu(fileName = "CurvedFire", menuName = "Spell/Attack/CurvedFire", order = 2)]
public class ShootProjectile:Attack
{
    public bool CurvedFire = true;

    public override void OnStartAttack(Transform target,Transform source)
    {
        Vector3 velocity = GetParabolicVelocity(source.position, target.position, _spellReference.element.standardVelocity, _spellReference.element.baseProjectile.gravity , 50f);
        CreateProjectile(_spellReference.element.baseProjectile.prefab,source.position,velocity, parameters.scale , _spellReference.element.baseProjectile.gravity, Mathf.RoundToInt(_spellReference.element.damage * parameters.damage) );
    }

}