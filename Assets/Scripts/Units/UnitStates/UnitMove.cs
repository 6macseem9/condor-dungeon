using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class UnitMove : UnitState
{
    private SpriteRenderer _actionMarker;
    private bool _startedPath;

    public UnitMove(Unit unit, Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker) : base(unit, animator, nav)
    {
        _actionMarker = actionMarker;
    }
    public override void OnEnter()
    {
        _animator.Play("walk");

        Util.Delay(0.04f, () => _startedPath = true);

        _nav.avoidancePriority = 49;
        _nav.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
    public override void Update()
    {
        _actionMarker.transform.position = _nav.destination;

        //if (!_unit.HoldPosition && _unit.AttackTarget != null)
        //{
        //    var enemy = _unit.AttackTarget.transform;
        //    var dir = _transform.position - enemy.position;
        //    var dest = enemy.position + dir.normalized;
        //    _nav.SetDestination(dest);
        //}

        if (_startedPath && _nav.velocity == Vector3.zero)
        {
            CompleteState();
        }
    }
    public override void FixedUpdate()
    {
        if (_nav.steeringTarget == _transform.position) return;
        _lookTarget = _nav.steeringTarget;
        base.FixedUpdate();
    }
    public override void OnExit()
    {
        _startedPath = false;
    }
}
