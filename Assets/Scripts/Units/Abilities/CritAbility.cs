using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritAbility : Ability
{
    [SerializeField] private int _chance = 15;

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
        if (Random.Range(1,101) <= _chance) 
        {
            _particles.Play();
            _unit.AttackTarget.TakeDamage(_unit);
        }
    }
}
