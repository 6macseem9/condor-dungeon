using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackSpellAbility : Ability
{
    [SerializeField] private int _maxStacks = 3;
    private int _stacks;

    private MagicUnit _magicUnit;

    private Spell _empoweredSpell;
    private Spell _defaultSpell;

    protected override void Start()
    {
        base.Start();
        _magicUnit = (_unit as MagicUnit);

        _empoweredSpell = GetComponentInChildren<Spell>();
        _defaultSpell = _magicUnit.Spell;

        _empoweredSpell.transform.localPosition = _magicUnit.SpellPosition;

        _magicUnit.OnAttack += StackCast;
    }

    private void StackCast()
    {
        _stacks++;
        if (_stacks > _maxStacks) _stacks = 1;
        
        if(_stacks==_maxStacks)
            _magicUnit.Spell = _empoweredSpell;
        else
            _magicUnit.Spell = _defaultSpell;
    }
}
