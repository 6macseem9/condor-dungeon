using UnityEngine;
using UnityEngine.AI;

public class UnitState : State
{
    protected Animator _animator;
    protected NavMeshAgent _nav;
    protected Transform _transform;

    public UnitState(Transform transform, Animator animator, NavMeshAgent nav)
    {
        _animator = animator;
        _nav = nav;
        _transform = transform;
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
