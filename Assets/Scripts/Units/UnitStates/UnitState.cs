using UnityEngine;
using UnityEngine.AI;

public class UnitState : State
{
    protected Unit _unit;
    protected Animator _animator;
    protected NavMeshAgent _nav;
    protected Transform _transform;

    public UnitState(Unit unit, Animator animator, NavMeshAgent nav)
    {
        _animator = animator;
        _nav = nav;
        _transform = unit.transform;
        _unit = unit;
    }

    public override void FixedUpdate()
    {
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
