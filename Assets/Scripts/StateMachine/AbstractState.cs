using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractState : MonoBehaviour
{
    public AbstractState[] transitions;
    public AbstractSubState[] subStates;
    public AbstractAction[] actions;

    virtual public bool IsStateReady(ref StateMachine stateMachine) { return false; }
    virtual public void OnStateEnter(ref StateMachine stateMachine, AbstractState previousState) { }
    virtual public void OnStateUpdate(ref StateMachine stateMachine) { }
    virtual public void OnStateFixedUpdate(ref StateMachine stateMachine) { }
    virtual public bool IsStateFinished() { return true; }
    virtual public void OnStateExit() { }
}
