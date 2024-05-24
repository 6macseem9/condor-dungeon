using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[DefaultExecutionOrder(2)]
public class Enemy : MonoBehaviour
{
    [SerializeField] private int _chanceToTargerFarthest;

    private Unit _unit;
    private EnemyClass _class;

    private List<Unit> _neighbors = new List<Unit>();

    private void Start()
    {
        _unit = GetComponent<Unit>();
        _class = _unit.Class as EnemyClass;

        _unit.DetectRange.OnEnter += AddNeighbor;
        _unit.DetectRange.OnExit += RemoveNeighbor;
        _unit.DetectRange.OnRetrigger += () => _neighbors.Clear();
    }

    private void ChooseTarget()
    {
        if (_neighbors.Count < 2 || _unit.IsDying) return;

        var index = Random.Range(1, 101) <= _chanceToTargerFarthest ? _neighbors.Count-1 : 0;
        var target = _neighbors.OrderBy(x => _unit.DistanceTo(x)).ToList()[index];
        _unit.Chase(target);
    }

    private void AddNeighbor(Unit unit)
    {
        _neighbors.Add(unit);

        ChooseTarget();
    }
    private void RemoveNeighbor(Unit unit)
    {
        _neighbors.Remove(unit);
        ChooseTarget();
    }

    public void LevelUp(int times)
    {
        for(int i = 0; i < times; i++)
        {
            Stats upgrade = _class.LevelUpBonuses.RandomChoice();
            _unit.BonusStats += upgrade;
            _unit.Level++;
        }
    }
}
