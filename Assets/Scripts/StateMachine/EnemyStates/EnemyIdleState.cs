using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : AbstractState
{
    private IntelligentEnemy _enemy;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        _enemy = GetComponentInParent<IntelligentEnemy>();

        _originalPosition = _enemy.transform.position;
        _originalRotation = _enemy.transform.rotation;
    }

    public override bool IsStateFinished()
    {
        return true;
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        return true;
    }

    public override void OnStateEnter(ref StateMachine stateMachine)
    {
        _enemy.navMeshAgent.destination = _originalPosition;
        _enemy.navMeshAgent.isStopped = false;
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
    {
    }

    public override void OnStateUpdate(ref StateMachine stateMachine)
    {
        if (_enemy.navMeshAgent.remainingDistance == 0.0f)
        {
            _enemy.transform.rotation = _originalRotation;
        }
    }
}
