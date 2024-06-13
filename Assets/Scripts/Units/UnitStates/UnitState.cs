using UnityEngine;
using UnityEngine.AI;

public class UnitState : State
{
    protected Unit _unit;
    protected Animator _animator;
    protected NavMeshAgent _nav;
    protected Transform _transform;
    protected Vector3 _lookTarget;

    public UnitState(Unit unit, Animator animator, NavMeshAgent nav)
    {
        _animator = animator;
        _nav = nav;
        _transform = unit.transform;
        _unit = unit;
    }

    public override void FixedUpdate()
    {
        Vector3 lookAtPos = _lookTarget - _transform.position;
        Quaternion newRotation = Quaternion.LookRotation(lookAtPos, _transform.up);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, newRotation, 0.1f);
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void Update()
    {
    }
}
