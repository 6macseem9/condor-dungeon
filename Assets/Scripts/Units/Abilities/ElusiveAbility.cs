using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElusiveAbility : Ability
{
    [SerializeField] int _chance;

    private Transform _model;

    protected override void Start()
    {
        base.Start();
        _unit.HandleDamage = Evade;

        _model = _unit.GetComponentInChildren<UnitAnimator>().transform;
    }

    private int Evade(int damage)
    {
        if (Random.Range(1, 101) <= _chance)
        {
            _model.DOLocalRotate(new Vector3(0, 540, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc);
            return 0;
        }
        else return damage;
    }

    
}
