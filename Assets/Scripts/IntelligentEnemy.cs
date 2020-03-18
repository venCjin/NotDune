using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IntelligentEnemy : StateMachine, IDamageable
{
    [System.Serializable]
    public class Parameters
    {
        public float distance = 10.0f;
        public float angle = 45.0f;
        public LayerMask layerMask = 0;

        public float angularAcceleration = 10.0f;
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

    private int _health = 100;

    public new Transform transform { get => _references.transform; }
    public GameObject states { get => _references.states; }
    public NavMeshAgent navMeshAgent { get => _references.navMeshAgent; }

    public bool canSeeCharacter { get; private set; }
    public Vector3 lastKnownChatacterPosition { get; private set; }

    public UnityAction<int> OnHealthChanged;

    private void Awake()
    {
        lastKnownChatacterPosition = new Vector3(float.NaN, float.NaN, float.NaN);

        _character = FindObjectOfType<CharacterController>();

        _character.OnHide += OnCharacterHide;
        _character.OnUnhide += OnCharacterUnhide;
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

        if (canSeeCharacter)
        {
            lastKnownChatacterPosition = _character.transform.position;
        }
        else
        {
            lastKnownChatacterPosition = new Vector3(float.NaN, float.NaN, float.NaN);
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

            if (canSeeCharacter == false)
            {
                canSeeCharacter = true;
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

        Physics.Raycast(ray, out RaycastHit hit, _parameters.distance);

        return (hit.transform == _character.transform);
    }

    private bool CanSeeBait()
    {
        if (_character.isGroundBait == false)
        {
            return false;
        }

        Vector3 direction = _character.tail.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);

        direction.y = 0.0f;
        if (Vector3.Angle(direction, transform.forward) > _parameters.angle)
        {
            return false;
        }

        Physics.Raycast(ray, out RaycastHit hit, _parameters.distance);

        return (hit.collider && hit.collider.name == _character.tail.name);
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
        Gizmos.DrawWireSphere(transform.position, _parameters.distance);
    }
}
