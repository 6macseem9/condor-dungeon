using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : Unit
{
    [SerializeField] private Projectile _projectile;

    private ObjectPool<Projectile> _pool;

    protected override void Start()
    {
        base.Start();

        _pool = new ObjectPool<Projectile> (
            (x) => !x.gameObject.activeSelf,
            () => Instantiate(_projectile,transform),
            (x) => { return; },
            (x) => x.gameObject.SetActive(false)
        );
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void DealDamageToTarget()
    {
        if (AttackTarget == null) return;

        var proj = _pool.GetObject();
        var target = AttackTarget;
        proj.SetTarget(AttackTarget.transform, ()=> target.TakeDamage(this));
    }
}
