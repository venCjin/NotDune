using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : AbstractState
{
    [System.Serializable]
    public class Parameters
    {
        public Transform searchArea = null;
        public float searchRadius = 15.0f;
    }


    [System.Serializable]
    public class References
    {
    }

    [Space()]
    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private Vector3 _targetPosition;

    private CharacterController _character;
    private IntelligentEnemy _enemy;
    private EnemyManager _manager;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
        _enemy = GetComponentInParent<IntelligentEnemy>();
        _manager = FindObjectOfType<EnemyManager>();
    }

    public override bool IsStateFinished()
    {
        return (_manager.canAnybodySeePlayer);
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        return (_manager.canAnybodySeePlayer == false);
    }

    public override void OnStateEnter(ref StateMachine stateMachine, AbstractState previousState)
    {
        Vector3 direction = Random.onUnitSphere;
        direction.y = 0.0f;
        direction.Normalize();

        float distance = Random.value * _parameters.searchRadius;

        _targetPosition = _parameters.searchArea.position + direction * distance;

        _enemy.navMeshAgent.destination = _targetPosition;
        _enemy.navMeshAgent.isStopped = false;
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
    {
        if (_enemy.navMeshAgent.remainingDistance < 1.5f)
        {
            Vector3 direction = Random.onUnitSphere;
            direction.y = 0.0f;
            direction.Normalize();

            float distance = Random.value * _parameters.searchRadius;
            _targetPosition = _parameters.searchArea.position + direction * distance;

            _enemy.navMeshAgent.destination = _targetPosition;
            _enemy.navMeshAgent.isStopped = false;
        }
    }

    public override void OnStateUpdate(ref StateMachine stateMachine)
    {
    }
}
