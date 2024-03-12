using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAttack : UnitState
{
    private SpriteRenderer _actionMarker;
    public UnitAttack(Unit unit, Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker) : base(unit, animator, nav)
    {
        _actionMarker = actionMarker;
    }
    public override void OnEnter()
    {
        _animator.CrossFade("attack", 0.4f);

        _nav.SetDestination(_transform.position);

        _nav.avoidancePriority = 50;
        _nav.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    }
    public override void Update()
    {
        _actionMarker.transform.position = _unit.AttackTarget.transform.position;
    }
    public override void FixedUpdate()
    {
        if (_unit.AttackTarget == null) return;
        Vector3 lookAtPos = _unit.AttackTarget.transform.position - _transform.position;
        Quaternion newRotation = Quaternion.LookRotation(lookAtPos, _transform.up);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, newRotation, 0.1f);
    }
    public override void OnExit()
    {

    }


}
