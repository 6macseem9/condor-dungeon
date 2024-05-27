using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : Item
{
    [Space(10)]
    public int HealPercent;

    public override void Activate()
    {
        GlobalEffects.Instance.HealParticles.Play();

        if(HealPercent==100)
        {
            UnitSelectionManager.Instance.FullHeal();
            return;
        }

        UnitSelectionManager.Instance.AllUnits.ForEach(x => x.HealPercent(HealPercent));
    }
}
