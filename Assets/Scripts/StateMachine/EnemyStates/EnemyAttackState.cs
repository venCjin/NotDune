using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : AbstractState
{
    [System.Serializable]
    public class Parameters
    {
        public float maxShotRange = 7.5f;
        public float minShotRange = 5.0f;
        public AnimationCurve accuracyOfDistance = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        public float shootCooldown = 0.6f;
    }


    [System.Serializable]
    public class References
    {
        public ParticleSystem shootParticle;
        public CinemachineImpulseSource impulseSource;
    }

    [Space()]
    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private CharacterController _character;
    private IntelligentEnemy _enemy;

    private float _lastShootTime = 0.0f;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
        _enemy = GetComponentInParent<IntelligentEnemy>();
    }

    public override bool IsStateFinished()
    {
        return (_enemy.canSeeCharacter == false && _enemy.navMeshAgent.remainingDistance == 0.0f);
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        return (_enemy.canSeeCharacter);
    }

    public override void OnStateEnter(ref StateMachine stateMachine)
    {

    }

    public override void OnStateExit()
    {
        _enemy.navMeshAgent.isStopped = true;
    }

    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
    {
        float distanceToDestination = Vector3.Distance(transform.position, _enemy.lastKnownChatacterPosition);

        if (distanceToDestination >= _parameters.minShotRange)
        {
            _enemy.navMeshAgent.isStopped = false;
            _enemy.navMeshAgent.SetDestination(_enemy.lastKnownChatacterPosition);
        }
        
        if (_enemy.canSeeCharacter)
        {
            if (distanceToDestination <= _parameters.maxShotRange)
            {
                float t = Mathf.InverseLerp(_parameters.maxShotRange, _parameters.minShotRange, distanceToDestination);
                float accuracy = _parameters.accuracyOfDistance.Evaluate(t);

                if (Time.time > _lastShootTime + _parameters.shootCooldown)
                {
                    Shoot(accuracy);
                }
            }

            if (distanceToDestination <= _parameters.minShotRange)
            {
                _enemy.navMeshAgent.isStopped = true;
            }
        }
    }

    public override void OnStateUpdate(ref StateMachine stateMachine)
    {

    }

    private void Shoot(float accuracy)
    {
        FindObjectOfType<CharacterController>().ReceiveDamage(5);
        _references.impulseSource.GenerateImpulse();

        _references.shootParticle.Play();
        _lastShootTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _parameters.maxShotRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _parameters.minShotRange);
    }
}
