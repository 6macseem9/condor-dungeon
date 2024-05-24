using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class ChivalryAbility : Ability
{
    [Space(7)]
    [SerializeField] private Stats _bonusForEach;

    private Stats _currentBonus;

    protected override void Start()
    {
        base.Start();

        _currentBonus = new Stats();

        Description = Description.Replace("X", _bonusForEach.Strength.ToString()).Replace("Y", _bonusForEach.Armor.ToString());

        UnitSelectionManager.Instance.UnitAddedOrRemoved += UpdateBonuses;

        UpdateBonuses(UnitSelectionManager.Instance.AllUnits,null);
    }

    private void UpdateBonuses(List<Unit> allUnits, Unit addedUnit)
    {
        _unit.BonusStats -= _currentBonus;

        var sameClassCount = allUnits.Count(x=>x.Class.ClassName == _unit.Class.ClassName) - 1 ;

        _currentBonus = _bonusForEach * sameClassCount;
        _unit.BonusStats += _currentBonus;
    }
}
