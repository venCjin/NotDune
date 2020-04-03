using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Linq;

public class IntelligentEnemy : StateMachine, IDamageable
{
    [System.Serializable]
    public class Parameters
    {
        public float angle = 45.0f;
        public LayerMask layerMask = 0;

        public float angularAcceleration = 10.0f;

        [Space()]

        public AnimationCurve TimeToDetectOverDistance = AnimationCurve.Linear(0.0f, 0.5f, 10.0f, 5.0f);
        public float TimeToForget = 2.0f;
    }

    [System.Serializable]
    public class References
    {
        public Transform transform;
        public GameObject states;
        public ColorController colorController;
        public GameObject geometry;
        public GameObject UI;
        public NavMeshAgent navMeshAgent;

        [Space()]
        public ParticleSystem rippleParticle;
        public ParticleSystem shootParticle;

        [Space()]
        public CinemachineImpulseSource impulseSource;
    }

    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private CharacterController _character;
    private EnemyManager _manager;

    private int _health = 100;

    private float _lastCharacterSeenTime = 0.0f;

    public new Transform transform
    {
        get => _references.transform;
    }
    public GameObject states
    {
        get => _references.states;
    }
    public NavMeshAgent navMeshAgent
    {
        get => _references.navMeshAgent;
    }
    public bool canSeeCharacter
    {
        get; private set;
    }
    public bool hasDetectedCharacter
    {
        get; private set;
    }
    public Vector3 lastKnownChatacterPosition
    {
        get; private set;
    }
    public float detectionLevel
    {
        get;
        private set;
    }

    public Vector3 originalPosition
    {
        get;
        private set;
    }

    public UnityAction<int> OnHealthChanged;

    private void Awake()
    {
        lastKnownChatacterPosition = new Vector3(float.NaN, float.NaN, float.NaN);

        _manager = FindObjectOfType<EnemyManager>();
        _character = FindObjectOfType<CharacterController>();

        _character.OnHide += OnCharacterHide;
        _character.OnUnhide += OnCharacterUnhide;

        originalPosition = transform.position;
    }

    private void OnDestroy()
    {
        _character.OnHide -= OnCharacterHide;
        _character.OnUnhide -= OnCharacterUnhide;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        canSeeCharacter = CanSeeCharacter();
        hasDetectedCharacter = HasDetectedEnemy();

        if (hasDetectedCharacter)
        {
            lastKnownChatacterPosition = _character.transform.position;
        }

        if (CanSeeBait())
        {
            lastKnownChatacterPosition = _character.transform.position;
            this.ChangePrimaryState(this.states.GetComponent<EnemySearchState>());
        }
    }

    private void OnCharacterUnhide()
    {
        _references.rippleParticle.Clear();
        _references.rippleParticle.Stop();

        _references.geometry.SetActive(true);
        _references.UI.SetActive(true);
    }

    private void OnCharacterHide()
    {
        _references.rippleParticle.Play();
        _references.geometry.SetActive(false);
        _references.UI.SetActive(false);
    }

    public void ReceiveDamage(int damage)
    {

        _health -= damage;
        OnHealthChanged?.Invoke(_health);

        if (_health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _references.colorController.HighLight();

            if (hasDetectedCharacter == false)
            {
                hasDetectedCharacter = true;
                lastKnownChatacterPosition = _character.transform.position;

                Vector3 forward = _character.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(forward);
            }
        }
    }

    private bool CanSeeCharacter()
    {
        if (_character.isAboveGround == false)
        {
            return false;
        }

        Vector3 direction = _character.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);

        direction.y = 0.0f;
        if (Vector3.Angle(direction, transform.forward) > _parameters.angle)
        {
            return false;
        }

        float distance = _parameters.TimeToDetectOverDistance.keys.Last().time;
        Physics.Raycast(ray, out RaycastHit hit, distance);

        if (hit.transform == _character.transform)
        {
            _lastCharacterSeenTime = Time.time;
            return true;
        }

        return false;
    }

    private bool CanSeeBait()
    {
        return false;

        //if (_character.isGroundBait == false)
        //{
        //    return false;
        //}

        //Vector3 direction = _character.tail.transform.position - transform.position;
        //Ray ray = new Ray(transform.position, direction);

        //direction.y = 0.0f;
        //if (Vector3.Angle(direction, transform.forward) > _parameters.angle)
        //{
        //    return false;
        //}

        //Physics.Raycast(ray, out RaycastHit hit, _parameters.distance);

        //return (hit.collider && hit.collider.name == _character.tail.name);
    }

    private bool HasDetectedEnemy()
    {
        if (IsCurrentlyInState(typeof(EnemyIdleState)) || IsCurrentlyInState(typeof(EnemyPatrolState)))
        {
            if (canSeeCharacter == true)
            {
                float distance = Vector3.Distance(transform.position, _character.transform.position);
                float speed = 1.0f / _parameters.TimeToDetectOverDistance.Evaluate(distance);

                detectionLevel += speed * Time.fixedDeltaTime;
            }
            else if (Time.time > _lastCharacterSeenTime + 1.0f)
            {
                float speed = 1.0f * _parameters.TimeToForget;
                detectionLevel -= speed * Time.fixedDeltaTime;
            }
        }
        else if (IsCurrentlyInState(typeof(EnemySearchState)))
        {
            detectionLevel = 1.0f;
        }
        else if (IsCurrentlyInState(typeof(EnemyAttackState)))
        {
            detectionLevel = 1.0f;
        }

        detectionLevel = Mathf.Clamp01(detectionLevel);

        return (canSeeCharacter && detectionLevel >= 1.0f);
    }

    private void OnDrawGizmos()
    {
        if (_character == null) { return; }

        if (canSeeCharacter)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _character.transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, _parameters.distance);

        float endDistance = _parameters.TimeToDetectOverDistance.keys.Last().time;
        float endValue = _parameters.TimeToDetectOverDistance.keys.Last().value;

        for (int i = 0; i < 100; i++)
        {
            float t = (1.0f * i / 100);
            Vector3 start = transform.position + t * transform.forward * endDistance;
            Vector3 ray = transform.up * _parameters.TimeToDetectOverDistance.Evaluate(t * endDistance) / endValue;
            Gizmos.DrawRay(start, ray);
        }
    }
}
