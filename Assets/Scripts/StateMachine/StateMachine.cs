using System;
using System.Collections.Generic;
using System.Diagnostics;

public class StateMachine
{
    private StateNode current;
    private Dictionary<Type, StateNode> nodes = new();
    private HashSet<Transition> anyTransitions = new();

    public string CurrentStateName => current.State.GetType().Name;
    public string PreviousStateName { get; private set; }

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);

        current.State?.Update();
    }

    public void FixedUpdate()
    {
        current.State?.FixedUpdate();
    }

    public void SetState(State state)
    {
        current = nodes[state.GetType()];
        current.State?.Enter();
    }

    private void ChangeState(State state)
    {
        if (state == current.State) return;

        var previousState = current.State;
        PreviousStateName = previousState.GetType().ToString();
        var nextState = nodes[state.GetType()].State;

        previousState?.Exit();
        nextState?.Enter();
        current = nodes[state.GetType()];
    }

    private Transition GetTransition()
    {
        foreach (var transition in anyTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in current.Transitions)
            if (transition.Condition())
                return transition;

        return null;
    }

    public void AddTransition(State from, State to, Func<bool> condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(State to, Func<bool> condition)
    {
        anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    private StateNode GetOrAddNode(State state)
    {
        var node = nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }

        return node;
    }

    private class StateNode
    {
        public State State { get; }
        public HashSet<Transition> Transitions { get; }

        public StateNode(State state)
        {
            State = state;
            Transitions = new HashSet<Transition>();
        }

        public void AddTransition(State to, Func<bool> condition)
        {
            Transitions.Add(new Transition(to, condition));
        }
    }
}