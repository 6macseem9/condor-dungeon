using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class UnitChase : UnitState
{
    private SpriteRenderer _actionMarker;

    public UnitChase(Unit unit, Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker) : base(unit, animator, nav)
    {
        _actionMarker = actionMarker;
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
        var dest = enemy.position + dir.normalized;
        _nav.SetDestination(dest);
    }
    public override void FixedUpdate()
    {
        if (_nav.steeringTarget == _transform.position) return;
        Vector3 lookAtPos = _nav.steeringTarget - _transform.position;
        Quaternion newRotation = Quaternion.LookRotation(lookAtPos, _transform.up);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, newRotation, 0.1f);
    }
    public override void OnExit()
    {

    }
}
