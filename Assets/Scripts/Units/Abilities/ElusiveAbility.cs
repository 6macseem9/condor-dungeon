using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElusiveAbility : Ability
{
    [Space(7)]
    [SerializeField] int _chance;
    [SerializeField] private Transform _model;

    protected override void Start()
    {
        base.Start();
        _unit.HandleDamage = Evade;
    }

    private int Evade(int damage)
    {
        if (Random.Range(1, 101) <= _chance)
        {
            _model.DOKill();
            _model.DOLocalRotate(Vector3.zero, 0);
            _model.DOLocalRotate(new Vector3(0, 360, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc);
            return 0;
        }
        else return damage;
    }

    
}
