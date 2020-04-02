using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class StateMachine : MonoBehaviour
{
    [SerializeField] public AbstractState _currentState;
    private List<AbstractSubState> _currentSubStates = new List<AbstractSubState>();

    public UnityAction<System.Type> OnStateChanged = null;

    protected virtual void Start()
    {
        if (_currentState != null) { ChangePrimaryState(_currentState); }
    }

    protected virtual void Update()
    {
        var stateMachine = this;

        foreach (var state in _currentState.transitions)
        {
            if (state == _currentState) { continue; }

            if (state.IsStateReady(ref stateMachine) && _currentState.IsStateFinished())
            {
                ChangePrimaryState(state);
                break;
            }
        }

        if (_currentState != null)
        {
            _currentState.OnStateUpdate(ref stateMachine);

            foreach (var action in _currentState.actions)
            {
                if (action.IsActionReady())
                {
                    action.OnActionPerformed();
                }
            }

            foreach (var substate in _currentState.subStates)
            {
                if (_currentSubStates.Contains(substate)) { continue; }

                if (substate.IsStateReady(ref stateMachine))
                {
                    _currentSubStates.Add(substate);
                    substate.OnStateEnter(ref stateMachine);
                }
            }

            for (int i = 0; i < _currentSubStates.Count; i++)
            {
                var substate = _currentSubStates[i];

                if (substate.IsStateFinished())
                {
                    _currentSubStates.Remove(substate);
                    substate.OnStateExit();
                }
                else
                {
                    substate.OnStateUpdate(ref stateMachine);
                }
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_currentState != null)
        {
            var stateMachine = this;
            _currentState.OnStateFixedUpdate(ref stateMachine);

            foreach (var substate in _currentSubStates)
            {
                substate.OnStateFixedUpdate(ref stateMachine);
            }
        }
    }

    public void ChangePrimaryState(AbstractState newState)
    {
        var previousState = _currentState;

        if (_currentState != null)
            _currentState.OnStateExit();

        foreach(var substate in _currentSubStates)
            substate.OnStateExit();

        _currentSubStates.Clear();


        _currentState = newState;

        var stateMachine = this;
        _currentState.OnStateEnter(ref stateMachine, previousState);

        OnStateChanged?.Invoke(_currentState.GetType());
    }

    public bool IsCurrentlyInState(Type state)
    {
        return (_currentState.GetType() == state);
    }

    public Type GetCurrentStateType()
    {
        return _currentState.GetType();
    }
}
