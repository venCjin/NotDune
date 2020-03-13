using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class StateMachine : MonoBehaviour
{
    [SerializeField] public AbstractState _currentState;

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
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_currentState != null)
        {
            var stateMachine = this;
            _currentState.OnStateFixedUpdate(ref stateMachine);
        }
    }

    public void ChangePrimaryState(AbstractState newState)
    {
        if (_currentState != null)
            _currentState.OnStateExit();

        _currentState = newState;

        var stateMachine = this;
        _currentState.OnStateEnter(ref stateMachine);

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
