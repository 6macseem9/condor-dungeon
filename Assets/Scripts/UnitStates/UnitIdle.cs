using UnityEngine;
using UnityEngine.AI;

public class UnitIdle : UnitState
{
    public UnitIdle(Unit unit, Animator animator, NavMeshAgent nav) : base(unit, animator, nav)
    {
    }
    public override void OnEnter()
    {
        _animator.CrossFade("idle", 0.4f);

        _nav.avoidancePriority = 50;
        _nav.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {

    }
    public override void OnExit()
    {

    }

    
}
