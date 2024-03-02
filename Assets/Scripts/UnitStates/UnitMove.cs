using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class UnitMove : UnitState
{
    private SpriteRenderer _actionMarker;
    private bool _startedPath;

    public UnitMove(Transform transform, Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker) : base(transform, animator, nav)
    {
        _actionMarker = actionMarker;
    }
    public override void OnEnter()
    {
        _animator.Play("walk");

        Util.Delay(0.02f, () => _startedPath = true);

        _nav.avoidancePriority = 49;
        _nav.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
    public override void Update()
    {
        _actionMarker.transform.position = _nav.destination;

        if (_startedPath && _nav.velocity == Vector3.zero)
        {
            CompleteState();
        }
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
        _actionMarker.enabled = false;
        _startedPath = false;
    }
}
