using UnityEngine;
using UnityEngine.AI;

public class UnitState : State
{
    protected Animator _animator;
    protected NavMeshAgent _nav;

    public UnitState(Animator animator, NavMeshAgent nav)
    {
        _animator = animator;
        _nav = nav;
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
