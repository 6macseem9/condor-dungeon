using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[DefaultExecutionOrder(2)]
public class Enemy : MonoBehaviour
{
    private Unit _unit;
    private Vector3 _targerPosition;

    private List<Unit> _neighbors = new List<Unit>();

    private void Start()
    {
        _unit = GetComponent<Unit>();
        _targerPosition = new Vector3(transform.position.x, transform.position.y, -3);

        _unit.DetectRange.OnEnter += AddNeighbor;
        _unit.DetectRange.OnExit += RemoveNeighbor;
        _unit.DetectRange.OnRetrigger += () => _neighbors.Clear();

        _unit.HoldPosition = false;
        //_unit.TargetLost += MoveToNexus;
        MoveToNexus();
    }

    private void FixedUpdate()
    {
        if (_neighbors.Count<2 || _unit.IsDying) return;
        
        foreach (var neighbor in _neighbors)
        {
            if (neighbor == _unit.AttackTarget) continue;

            var distance = _unit.DistanceTo(neighbor);
            if (distance==0) continue;

            if (distance < _unit.DistanceTo(_unit.AttackTarget))
            {
                _unit.Chase(neighbor);
            }
        }
    }

    private void MoveToNexus()
    {
        _unit.MoveTo(_targerPosition);
    }

    private void AddNeighbor(Unit unit)
    {
        _neighbors.Add(unit);
    }
    private void RemoveNeighbor(Unit unit)
    {
        _neighbors.Remove(unit);
    }
}
