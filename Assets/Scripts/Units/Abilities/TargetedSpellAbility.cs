using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedSpellAbility : Ability
{
    private MagicUnit _magicUnit;

    protected override void Start()
    {
        base.Start();
        _magicUnit = (_unit as MagicUnit);

        _magicUnit.OnAttack += MoveSpellToTarget;
    }

    private void MoveSpellToTarget()
    {
        _magicUnit.Spell.transform.position = _magicUnit.AttackTarget.transform.position;
    }
}
