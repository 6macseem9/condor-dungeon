using System;
using System.Collections.Generic;
using System.Diagnostics;

public class StateMachine
{
    private StateNode _current;
    private Dictionary<Type, StateNode> _nodes = new();

    public string CurrentStateName => _current.State.GetType().Name;
    public string PreviousStateName { get; private set; }

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
            ChangeState(transition.To);

        _current.State?.Update();
    }

    public void FixedUpdate()
    {
        _current.State?.FixedUpdate();
    }

    public void TransitionTo(State state)
    {
        if(_current==null)
        {
            _current = _nodes[state.GetType()];
            _current.State?.Enter();
            return;
        }

        ChangeState(state);
    }

    private void ChangeState(State state)
    {
        if (state == _current.State) return;

        var previousState = _current.State;
        PreviousStateName = previousState.GetType().ToString();
        var nextState = _nodes[state.GetType()].State;

        previousState?.Exit();
        nextState?.Enter();
        _current = _nodes[state.GetType()];
    }

    private Transition GetTransition()
    {
        foreach (var transition in _current.Transitions)
            if (transition.Condition())
                return transition;

        return null;
    }

    public void AddTransition(State from, State to, Func<bool> condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    private StateNode GetOrAddNode(State state)
    {
        var node = _nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            _nodes.Add(state.GetType(), node);
        }

        return node;
    }

    public void AddNode(State state)
    {
        GetOrAddNode(state);
    }

    private class StateNode
    {
        public State State;
        public List<Transition> Transitions;

        public StateNode(State state)
        {
            State = state;
            Transitions = new List<Transition>();
        }

        public void AddTransition(State to, Func<bool> condition)
        {
            Transitions.Add(new Transition(to, condition));
        }
    }
}