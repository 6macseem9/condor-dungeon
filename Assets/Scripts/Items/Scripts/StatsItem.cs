using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatsItem : Item
{
    [Space(10)]
    public float Time;
    public bool Negative;
    public Stats Stats;

    public override void Activate()
    {
        if (Negative)
        {
            GlobalEffects.Instance.DebuffParticles.Play();
            Debuff();
        }
        else 
        {
            GlobalEffects.Instance.BuffParticles.Play();
            Buff(); 
        }
    }

    private void Buff()
    {
        var allUnits = UnitSelectionManager.Instance.AllUnits;
        allUnits.ForEach(unit => { unit.BonusStats += Stats; unit.UpdateAttackSpeed(); });
        UnitSelectionManager.Instance.UpdateSelectedUnitStats();

        Util.Delay(Time, () =>
        {
            allUnits.ForEach(unit => { unit.BonusStats -= Stats; unit.UpdateAttackSpeed(); });
            UnitSelectionManager.Instance.UpdateSelectedUnitStats();
        });
    }
    private void Debuff()
    {
        var enemies = FindObjectsOfType<Unit>().Where(x => x.IsEnemy).ToList();
        enemies.ForEach(unit => { unit.BonusStats -= Stats; unit.UpdateAttackSpeed(); });
        UnitSelectionManager.Instance.UpdateSelectedUnitStats();

        Util.Delay(Time, () =>
        {
            enemies.ForEach(unit => { unit.BonusStats += Stats; unit.UpdateAttackSpeed(); });
            UnitSelectionManager.Instance.UpdateSelectedUnitStats();
        });
    }
}
