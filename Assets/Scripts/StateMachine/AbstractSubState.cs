using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractSubState : MonoBehaviour
{
    virtual public bool IsStateReady(ref StateMachine stateMachine) { return false; }
    virtual public void OnStateEnter(ref StateMachine stateMachine) { }
    virtual public void OnStateUpdate(ref StateMachine stateMachine) { }
    virtual public void OnStateFixedUpdate(ref StateMachine stateMachine) { }
    virtual public bool IsStateFinished() { return true; }
    virtual public void OnStateExit() { }
}
