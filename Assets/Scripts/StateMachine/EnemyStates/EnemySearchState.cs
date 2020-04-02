using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchState : AbstractState
{
    [System.Serializable]
    public class Parameters
    {
        public float duration;

        public float minDistance = 5.0f;
        public float maxDistance = 10.0f;
    }


    [System.Serializable]
    public class References
    {

    }

    [Space()]
    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private Vector3 _targetPosition;
    private float _behaviourTime = 0.0f;
    private float _waitingTime = 0.0f;

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
        return (_behaviourTime > _parameters.duration || _enemy.canSeeCharacter);
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        return (_manager.canAnybodySeePlayer);
    }

    public override void OnStateEnter(ref StateMachine stateMachine, AbstractState previousState)
    {
        _behaviourTime = 0.0f;

        Vector3 direction = Random.onUnitSphere;
        direction.y = 0.0f;
        direction.Normalize();

        float distance = Random.Range(_parameters.minDistance, _parameters.maxDistance);

        if (float.IsNaN(_enemy.lastKnownChatacterPosition.sqrMagnitude))
        {
            _targetPosition = _manager.lastKnownChatacterPosition + direction * distance;
        }
        else
        {
            _targetPosition = _enemy.lastKnownChatacterPosition + direction * distance;
        }

        _enemy.navMeshAgent.destination = _targetPosition;
        _enemy.navMeshAgent.isStopped = false;
    }

    public override void OnStateExit()
    {
        _behaviourTime = 0.0f;
    }

    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
    {
        if (_enemy.navMeshAgent.remainingDistance < 1.0f)
        {
            _enemy.navMeshAgent.isStopped = false;
            if (_waitingTime > 3.0f)
            {
                _waitingTime = 0.0f;

                Vector3 direction = Random.onUnitSphere;
                direction.y = 0.0f;
                direction.Normalize();

                float distance = Random.Range(_parameters.minDistance, _parameters.maxDistance);

                _targetPosition = _enemy.transform.position + direction * distance;
                _enemy.navMeshAgent.destination = _targetPosition;
            }
            else
            {
                _waitingTime += Time.fixedDeltaTime;
            }
        }
    }

    public override void OnStateUpdate(ref StateMachine stateMachine)
    {
        _behaviourTime += Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (_behaviourTime == 0.0f) { return; }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_targetPosition, 0.5f);
    }
}
