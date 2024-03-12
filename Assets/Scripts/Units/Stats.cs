using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Unit Stats", order = 1)]
public class Stats : ScriptableObject
{
    public string ClassName;
    public Color ClassColor;
    public Sprite Portrait;
    [Space(7)]
    public int MaxHP;
    public float Regen;
    [Space(7)]
    public float AttackSpeed = 1;
    public float MoveSpeed;
    [Space(7)]
    public int MeleeDamage;
    public int MeleeResist;
    [Space(7)]
    public int RangedDamage;
    public int RangedResist;
    [Space(7)]
    public int MagicDamage;
    public int MagicResist;

    public int CalculateMitigatedDamage(Stats damageStats)
    {
        var melee = (damageStats.MeleeDamage - MeleeResist);
        melee = melee <= 0 ? 0 : melee;
        var ranged = (damageStats.RangedDamage - RangedResist);
        ranged = ranged <= 0 ? 0 : ranged;
        var magic = (damageStats.MagicDamage - MagicResist);
        magic = magic <= 0 ? 0 : magic;

        return melee + ranged + magic;
    }

    public float[] GetArray()
    {
        return new float[] { MaxHP, Regen, AttackSpeed, MoveSpeed, MeleeDamage, RangedDamage, MagicDamage, MeleeResist, RangedResist, MagicResist };
    }
}
