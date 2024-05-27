using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class RegenAbility : Ability
{
    [SerializeField] private int _percent;

    private Tweener _regen;
    private ParticleSystem _particles;

    protected override void Start()
    {
        base.Start();

        _particles = GetComponentInChildren<ParticleSystem>();

        _unit.OnDeath += () => _regen.Kill();

        _regen = Util.Repeat(1f,-1, () => 
        {
            var missingHp = _unit.Stats.MaxHP - _unit.HP;
            var missingPercent = missingHp * 100 / 300;

            var em = _particles.emission.GetBurst(0);
            em.count = missingPercent/10*3;
            _particles.emission.SetBurst(0, em);
            _particles.Play();

            int heal = (int)(missingHp * (_percent/100f)); 
            _unit.Heal(heal); 
        });
    }
}
