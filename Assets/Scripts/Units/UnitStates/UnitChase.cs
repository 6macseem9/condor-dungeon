using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class UnitChase : UnitState
{
    private SpriteRenderer _actionMarker;
    private Range _attackRange;

    public UnitChase(Unit unit, Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker, Range attackRange) : base(unit, animator, nav)
    {
        _actionMarker = actionMarker;
        _attackRange = attackRange;
    }
    public override void OnEnter()
    {
        _animator.Play("walk");

        _nav.avoidancePriority = 49;
        _nav.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
    public override void Update()
    {
        if (_unit.AttackTarget == null) return;
            
        _actionMarker.transform.position = _unit.AttackTarget.transform.position;

        var enemy = _unit.AttackTarget.transform;
        var dir = _transform.position - enemy.position;
        var dest = enemy.position + dir.normalized * (_attackRange.Radius + 0.3f);
        _nav.SetDestination(dest);
    }
    public override void FixedUpdate()
    {
        if (_nav.steeringTarget == _transform.position) return;
        _lookTarget = _nav.steeringTarget;
        base.FixedUpdate();
    }
}
