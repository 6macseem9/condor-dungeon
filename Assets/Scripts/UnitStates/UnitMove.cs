using UnityEngine;
using UnityEngine.AI;

public class UnitMove : UnitState
{
    private SpriteRenderer _actionMarker;

    public UnitMove(Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker) : base(animator, nav)
    {
        _actionMarker = actionMarker;
    }
    public override void OnEnter()
    {

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
