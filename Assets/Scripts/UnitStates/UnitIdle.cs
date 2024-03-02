using UnityEngine;
using UnityEngine.AI;

public class UnitIdle : UnitState
{
    public UnitIdle(Transform transform, Animator animator, NavMeshAgent nav) : base(transform,animator, nav)
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
