using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : Unit
{
    [SerializeField] private Projectile _projectile;

    private ObjectPool<Projectile> _pool;

    public Action OnHit;

    protected override void Start()
    {
        base.Start();

        _pool = new ObjectPool<Projectile> (
            (x) => !x.gameObject.activeSelf,
            () => Instantiate(_projectile,transform),
            (x) => { return; },
            (x) => x.gameObject.SetActive(false)
        );

        _pool.AddDefault(GetComponentInChildren<Projectile>());
    }
    public override void DealDamage()
    {
        var proj = _pool.GetObject();
        var target = AttackTarget;
        proj.SetTarget(AttackTarget.transform, ()=> { target.TakeDamage(this); OnHit.Invoke(); });
    }
}
