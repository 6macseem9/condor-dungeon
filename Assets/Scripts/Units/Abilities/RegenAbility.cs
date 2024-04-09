using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class RegenAbility : Ability
{
    [SerializeField] private float _delay;

    private ParticleSystem _particles;
    private float _defaultRegen;
    private Tweener _timer;

    protected override void Start()
    {
        base.Start();

        _particles = GetComponentInChildren<ParticleSystem>();

        _defaultRegen = _unit.Stats.Regen;
        _unit.DetectRange.OnEnter += Deactivate;
        _unit.DetectRange.OnExit += Activate;
        _unit.DetectRange.NoOneDetected += Activate;

        Activate();
    }

    private void Deactivate(Unit unit = null)
    {
        _timer.Kill();
        _unit.Stats.Regen = _defaultRegen;
        _particles.Stop();
    }
    private void Activate(Unit unit=null)
    {
        _timer = Util.Delay(_delay, () => 
        {
            _unit.Stats.Regen = _unit.Stats.Regen * 1.5f;
            _particles.Play();
        }) ;
    }
}
