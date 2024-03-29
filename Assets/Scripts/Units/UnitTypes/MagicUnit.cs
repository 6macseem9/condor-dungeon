using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class MagicUnit : Unit
{
    [SerializeField] public Spell Spell;
    [field: SerializeField] public Vector3 SpellPosition { get; private set; }

    public override void DealDamage()
    {
        Spell.Cast();
    }
}
