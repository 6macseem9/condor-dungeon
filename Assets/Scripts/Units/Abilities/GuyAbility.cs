using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuyAbility : Ability
{
    protected override void Start()
    {
        base.Start();
        _unit.LevelCost /= 2;
    }
}
