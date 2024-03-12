using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    public override void DealDamageToTarget()
    {
        if (AttackTarget == null) return;

        AttackTarget.TakeDamage(this);
    }
}
