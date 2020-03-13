using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractState : MonoBehaviour
{
    public AbstractState[] transitions;

    virtual public bool IsStateReady(ref StateMachine stateMachine) { return false; }
    virtual public void OnStateEnter(ref StateMachine stateMachine) { }
    virtual public void OnStateUpdate(ref StateMachine stateMachine) { }
    virtual public void OnStateFixedUpdate(ref StateMachine stateMachine) { }
    virtual public void OnStateExit() { }
    virtual public bool IsStateFinished() { return true; }
}
