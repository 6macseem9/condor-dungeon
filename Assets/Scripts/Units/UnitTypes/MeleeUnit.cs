using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    public override void DealDamage()
    {
        AttackTarget.TakeDamage(this);
    }
}
