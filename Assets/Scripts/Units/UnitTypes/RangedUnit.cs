using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : Unit
{
    [SerializeField] private Transform _projectile;
    private Vector3 _projectileDefPos;


    protected override void Start()
    {
        base.Start();

        _projectileDefPos = _projectile.localPosition;
        _projectile.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (AttackTarget == null) return;
        if (_projectile.gameObject.activeSelf)
        {
            _projectile.position = Vector3.MoveTowards(_projectile.position, AttackTarget.transform.position,10 * Time.deltaTime);
        }
    }

    public override void DealDamageToTarget()
    {
        if (AttackTarget == null) return;

        _projectile.localPosition = _projectileDefPos;
        _projectile.gameObject.SetActive(true);

        //AttackTarget.TakeDamage(this);
    }
}
