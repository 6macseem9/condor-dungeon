using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : Item
{
    [Space(10)]
    public Stats Stats;

    public override void Initiate()
    {
        UnitSelectionManager.Instance.UnitAddedOrRemoved += ApplyBuff;

        var allUnits = UnitSelectionManager.Instance.AllUnits;
        allUnits.ForEach(unit => { unit.BonusStats += Stats; unit.UpdateAttackSpeed(); });
        UnitSelectionManager.Instance.UpdateSelectedUnitStats();
    }
    public override void OnDiscard()
    {
        UnitSelectionManager.Instance.UnitAddedOrRemoved -= ApplyBuff;

        var allUnits = UnitSelectionManager.Instance.AllUnits;
        allUnits.ForEach(unit => { unit.BonusStats -= Stats; unit.UpdateAttackSpeed(); });
        UnitSelectionManager.Instance.UpdateSelectedUnitStats();
    }

    private void ApplyBuff(List<Unit> allUnits, Unit addedUnit)
    {
        addedUnit.BonusStats += Stats; 
        addedUnit.UpdateAttackSpeed();
        UnitSelectionManager.Instance.UpdateSelectedUnitStats();
    }
}
