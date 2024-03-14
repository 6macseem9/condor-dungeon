using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagicUnit : Unit
{
    [SerializeField] private Spell _basicSpell;
    [SerializeField] private Spell _powerfulSpell;

    private int _count;

    public override void DealDamageToTarget()
    {
        if (AttackTarget == null) return;

        _count++;
        if(_count==3)
        {
            _powerfulSpell.Cast();
            _count = 0;
        }
        else _basicSpell.Cast();
    }
}
