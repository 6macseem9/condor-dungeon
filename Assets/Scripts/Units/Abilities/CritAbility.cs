using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritAbility : Ability
{
    private ParticleSystem _particles;

    protected override void Start()
    {
        base.Start();

        _particles = GetComponentInChildren<ParticleSystem>();

        if (_unit is RangedUnit) (_unit as RangedUnit).OnHit += TryCrit;
        else _unit.OnAttack += TryCrit;
    }

    private void TryCrit()
    {
        if (Random.Range(1,101) <= _unit.Stats.Intellect) 
        {
            _particles.Play();
            if (_unit.AttackTarget != null) _unit.AttackTarget.TakeDamage(_unit);
        }
    }
}
