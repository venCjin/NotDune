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
        public int maxDamage = 15;
        public float maxShotRange = 7.5f;
        public float minShotRange = 5.0f;
        public AnimationCurve accuracyOfDistance = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        public float shootCooldown = 0.6f;
        internal float angularAcceleration = 5.0f;

        public float maxAngle = 15.0f;
        public AnimationCurve accuracyOfAngle = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
    }


    [System.Serializable]
    public class References
    {
        public ParticleSystem shootParticle;
        public CinemachineImpulseSource impulseSource;
        public LineRenderer shootLine;
    }

    [Space()]
    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private CharacterController _character;
    private IntelligentEnemy _enemy;
    private EnemyManager _manager;

    private float _lastShootTime = 0.0f;

    private float _originalAngularSpeed;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
        _enemy = GetComponentInParent<IntelligentEnemy>();
        _manager = FindObjectOfType<EnemyManager>();
    }

    public override bool IsStateFinished()
    {
        return (_manager.hasAnybodyDetectedPlayer == false);
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        return (_manager.hasAnybodyDetectedPlayer);
    }

    public override void OnStateEnter(ref StateMachine stateMachine, AbstractState previousState)
    {
        _originalAngularSpeed = _enemy.navMeshAgent.angularSpeed;
        _enemy.navMeshAgent.angularSpeed = 0.0f;

        /// Temporary solution to lock shooting for a little moment in situation when we attack from behind and Enemy turnes back and immediately starts shooting.
        _lastShootTime = Time.time;
    }

    public override void OnStateExit()
    {
        _enemy.navMeshAgent.isStopped = true;

        _enemy.navMeshAgent.angularSpeed = _originalAngularSpeed;
    }

    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
    {
        float distanceToDestination = Vector3.Distance(transform.position, _manager.lastKnownChatacterPosition);

        if (distanceToDestination >= _parameters.minShotRange)
        {
            _enemy.navMeshAgent.isStopped = false;
            _enemy.navMeshAgent.SetDestination(_manager.lastKnownChatacterPosition);
        }
        
        if (_enemy.canSeeCharacter)
        {
            if (distanceToDestination <= _parameters.maxShotRange)
            {
                float t = Mathf.InverseLerp(_parameters.maxShotRange, _parameters.minShotRange, distanceToDestination);
                float accuracyDistanceFactor = _parameters.accuracyOfDistance.Evaluate(t);

                Vector3 forward = _enemy.transform.forward; forward.y = 0.0f;
                Vector3 offset = _character.transform.position - _enemy.transform.position; offset.y = 0.0f;
                float angle = Vector3.Angle(forward, offset);

                t = Mathf.InverseLerp(0, _parameters.maxAngle, angle);
                float accuracyAngleDactor = _parameters.accuracyOfAngle.Evaluate(t);

                if (Time.time > _lastShootTime + _parameters.shootCooldown)
                {
                    float a = Mathf.Max(accuracyDistanceFactor, accuracyAngleDactor) * Mathf.Min(accuracyDistanceFactor, accuracyAngleDactor);
                    Shoot(a);
                }
            }

            if (distanceToDestination <= _parameters.minShotRange)
            {
                _enemy.navMeshAgent.isStopped = true;
            }

            _enemy.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(_enemy.transform.forward, (_character.transform.position - _enemy.transform.position), _parameters.angularAcceleration * Time.fixedDeltaTime));
        }
        else
        {
            _enemy.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(_enemy.transform.forward, _enemy.navMeshAgent.velocity, _parameters.angularAcceleration * Time.fixedDeltaTime));
        }
    }

    public override void OnStateUpdate(ref StateMachine stateMachine)
    {

    }

    private void Shoot(float accuracy)
    {
        _references.shootParticle.Play();
        _lastShootTime = Time.time;
        StartCoroutine(ShowShootLine());

        if (UnityEngine.Random.value < accuracy)
        {
            FindObjectOfType<CharacterController>().ReceiveDamage((int)(accuracy * _parameters.maxDamage));
            _references.impulseSource.GenerateImpulse();
        }
    }

    private IEnumerator ShowShootLine()
    {
        _references.shootLine.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        _references.shootLine.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _parameters.maxShotRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _parameters.minShotRange);
    }
}
