using System.Collections;
using System.Collections.Generic;
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

    //public int CalculateMitigatedDamage(Stats damageStats)
    //{
    //    var phys = (damageStats.Strength - Armor);
    //    phys = phys <= 0 ? 0 : phys;

    //    var magic = (damageStats.Intellect - Resistance);
    //    magic = magic <= 0 ? 0 : magic;

    //    return phys + magic;
    //}
    public int CalculateMitigatedDamage(Stats damageStats)
    {
        int damage;
        
        if(damageStats.Strength==0) damage = (damageStats.Intellect - Resistance);
        else damage = (damageStats.Strength - Armor);

        damage = damage <= 0 ? 0 : damage;

        return damage;
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

    public static Stats operator *(Stats a, int b)
       => new Stats(a.MaxHP * b,
           a.AttackSpeed * b,
           a.Strength * b,
           a.Armor * b,
           a.Intellect * b,
           a.Resistance * b);
    public static Stats operator /(Stats a, int b)
       => new Stats(a.MaxHP / b,
           a.AttackSpeed / b,
           a.Strength / b,
           a.Armor / b,
           a.Intellect / b,
           a.Resistance / b);
}
