using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public int MaxHP;
    public float AttackSpeed = 1;
    [Space(7)]
    public int Strength;
    public int Armor;
    [Space(7)]
    public int Intellect;
    public int Resistance;

    public Stats(int maxHP, float attackSpeed, int strength, int armor, int intellect, int resistance)
    {
        MaxHP = maxHP;
        AttackSpeed = attackSpeed;
        Strength = strength;
        Armor = armor;
        Intellect = intellect;
        Resistance = resistance;
    }

    public Stats()
    {
        MaxHP = 0;
        AttackSpeed = 0;
        Strength = 0;
        Armor = 0;
        Intellect = 0;
        Resistance = 0;
    }

    public int CalculateMitigatedDamage(Stats damageStats)
    {
        var melee = (damageStats.Strength - Armor);
        melee = melee <= 0 ? 0 : melee;
        var magic = (damageStats.Intellect - Resistance);
        magic = magic <= 0 ? 0 : magic;

        return melee + magic;
    }

    public List<float> GetArray()
    {
        return new List<float> { MaxHP, AttackSpeed, Strength, Armor, Intellect, Resistance };
    }


    public static Stats operator +(Stats a, Stats b)
       => new Stats(a.MaxHP + b.MaxHP,
           a.AttackSpeed + b.AttackSpeed,
           a.Strength + b.Strength,
           a.Armor + b.Armor,
           a.Intellect + b.Intellect,
           a.Resistance + b.Resistance);

    public static Stats operator -(Stats a, Stats b)
        => new Stats(a.MaxHP - b.MaxHP,
           a.AttackSpeed - b.AttackSpeed,
           a.Strength - b.Strength,
           a.Armor - b.Armor,
           a.Intellect - b.Intellect,
           a.Resistance - b.Resistance);
}

[CreateAssetMenu(fileName = "Class", menuName = "Unit Class", order = 1)]
public class Class : ScriptableObject
{
    public string ClassName;
    [TextArea]public string ClassDescription;
    public Color ClassColor;
    public AnimationClip Turnaround;
    [Space(7)]

    public Stats Stats;
}
