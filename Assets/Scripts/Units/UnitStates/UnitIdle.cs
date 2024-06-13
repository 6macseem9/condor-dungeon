using UnityEngine;
using UnityEngine.AI;

public class UnitIdle : UnitState
{
    private SpriteRenderer _actionMarker;
    public UnitIdle(Unit unit, Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker) : base(unit, animator, nav)
    {
        _actionMarker = actionMarker;
    }
    public override void OnEnter()
    {
        _actionMarker.enabled = false;
        _animator.CrossFade("idle", 0.4f);

        _nav.avoidancePriority = 50;
        _nav.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

        //if (_unit.HoldPosition) _unit.AttackTarget = null; //????
    }
    public override void FixedUpdate()
    {

    }

}
