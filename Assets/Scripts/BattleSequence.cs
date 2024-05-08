using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Battle
{
    public Unit[] Enemies;
    public int Amount;
    [Space(4)]
    public float Interval;
    public int SkipChance;
}

[CreateAssetMenu(fileName = "Battles", menuName = "Battle Sequence", order = 2)]
public class BattleSequence : ScriptableObject
{
    public Battle[] Sequence;
}
