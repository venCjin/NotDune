using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{

    public enum State { AboveGround, UnderGround, None };
    public State _state = State.None;
    public UnityAction<State> OnStateChanged = null;

    public Transform t;
    public Rigidbody rb;

    public CapsuleCollider capsuleCollider;

    [SerializeField] private float _aboveGroundSpeed = 5f;
    [SerializeField] private float _underGroundSpeed = 12f;

    [SerializeField] private float _currentMaxSpeed = 0f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _deceleration = 20f;

    private Vector2 _currentInput = Vector2.zero;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _currentDirection = Vector3.zero;

    // ATTACK
    [SerializeField] private bool _isAttacking = false;
    [SerializeField] private bool _canAttack = true;
    [SerializeField] private float _attackTime = 0.1f;
    [SerializeField] private float _attackCooldown = 0.1f;
    [SerializeField] private float _attackMaxSpeed = 30f;
    [SerializeField] private float _attackAcceleration = 40f;
    [SerializeField] private float _aboveGroundAttackRadius = 2.5f;
    [SerializeField] private float _underGroundAttackRadius = 3f;
    [SerializeField] GameObject _attackedEnemy = null;


    [SerializeField] private float _goTime = 0f;
    private void Start()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        ChangeState(State.AboveGround);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_state == State.AboveGround)
                Attack();

            ChangeState(State.AboveGround);
        }
        if (Input.GetMouseButtonDown(1))
        {
            ChangeState(State.UnderGround);
        }

    }
    private void FixedUpdate()
    {

        _currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Getting camera local axes
        var forward = Camera.main.transform.forward;
        forward = Vector3.ProjectOnPlane(forward, Vector3.up);
        forward.Normalize();

        var right = Camera.main.transform.right;
        right = Vector3.ProjectOnPlane(right, Vector3.up);
        right.Normalize();

        // Creating direction vector relative to camera's local axes
        _currentDirection = forward * _currentInput.y + right * _currentInput.x;
        _currentDirection.Normalize();

        // Calculating speed vector (acceleration and deceleraion) and rotation of player
        if (!_isAttacking)
        {

            if (_currentInput.x != 0 || _currentInput.y != 0)
            {
                t.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(new Vector3(_currentDirection.x, 0f, _currentDirection.z)), _acceleration * Time.fixedDeltaTime);
                _currentVelocity = Vector3.Lerp(_currentVelocity, _currentDirection * _currentMaxSpeed, _acceleration * Time.fixedDeltaTime);
            }
            else
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, _deceleration * Time.fixedDeltaTime);
            }

        }
        rb.velocity = new Vector3(_currentVelocity.x, rb.velocity.y, _currentVelocity.z);

    }


    private void ChangeState(State state)
    {
        if (_state == state)
            return;
        else
        {
            bool changeState = false;

            if (state == State.UnderGround)
                changeState = GoUnderGround();
            else
                changeState = GoAboveGround();

            if (changeState)
            {
                _state = state;
                OnStateChanged?.Invoke(state);
            }
        }

    }

    private bool GoUnderGround()
    {
        if (_state == State.UnderGround) return false;

        //rb.transform.position -= 1.25f * Vector3.up;
        //rb.transform.position = new Vector3(rb.transform.position.x, -1f, rb.transform.position.z);
        StartCoroutine(GoCoroutine(Vector3.up * -1.5f));
        //t.position = new Vector3(t.position.x, -1f, t.position.z);

        rb.velocity = Vector3.zero;
        //rb.useGravity = false;

        _currentMaxSpeed = _underGroundSpeed;
        return true;
    }

    private bool GoAboveGround()
    {
        if (_state == State.AboveGround) return false;

        RaycastHit[] hits;
        hits = Physics.RaycastAll(t.position, Vector3.up, 5f);
        
        foreach (RaycastHit rh in hits)
        {
            if (rh.collider.CompareTag("Obstacle"))
            {
                Debug.Log("Can't go Above Ground - obstacle above");
                return false;
            }
        }

        bool attack = false;
        foreach (EnemyAI enemy in GameManager.instance.enemiesList)
        {
            if (Vector3.Distance(new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z), new Vector3(t.position.x, 0f, t.position.z)) < _underGroundAttackRadius)
            {
                _attackedEnemy = enemy.gameObject;
                attack = true;
                break;
            }
        }
        if (attack)
            StartCoroutine(AttackFromUndergroundCoroutine(_attackedEnemy.transform));
        else
            StartCoroutine(GoCoroutine(Vector3.up * 1.5f));

        //t.position = new Vector3(t.position.x, 0.5f, t.position.z);


        rb.velocity = Vector3.zero;
        //rb.useGravity = true;

        _currentMaxSpeed = _aboveGroundSpeed;
        return true;
    }

    private void Attack()
    {
        Vector3 attackDirection = Vector3.zero;
        Transform enemyTransform = null;
        float previousDistance = _aboveGroundAttackRadius * 2f;
        float distance = 0f;
        bool foundEnemy = false;

        foreach (EnemyAI enemy in GameManager.instance.enemiesList)
        {
            distance = attackDirection.magnitude;
            attackDirection = (enemy.transform.position - t.position).normalized;

            if (Vector3.Dot(attackDirection, t.forward) > 0.55f && distance < _aboveGroundAttackRadius)
            {
                if (distance < previousDistance)
                {
                    enemyTransform = enemy.transform;
                    previousDistance = distance;
                    foundEnemy = true;
                }
                //_attackedEnemy = enemy.gameObject;
            }
        }
        if (foundEnemy)
            StartCoroutine(AttackCoroutine(attackDirection, enemyTransform));
        else
        {
            attackDirection = t.forward;
            StartCoroutine(AttackCoroutine(attackDirection, null));
        }


    }

    private IEnumerator AttackCoroutine(Vector3 attackDirection, Transform enemyTransform)
    {
        float timer = 0;
        _isAttacking = true;
        _currentVelocity = Vector3.zero;

        while (timer < _attackTime)
        {
            if (enemyTransform)
            {
                attackDirection = (enemyTransform.position - t.position).normalized;
                t.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(attackDirection), _attackAcceleration * Time.fixedDeltaTime);
            }

            _currentVelocity = Vector3.Lerp(_currentVelocity, attackDirection * _attackMaxSpeed, _attackAcceleration * Time.fixedDeltaTime);

            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        _currentVelocity *= 0.2f;
        _attackedEnemy = null;
        _isAttacking = false;
    }

    private IEnumerator AttackFromUndergroundCoroutine(Transform enemyTransform)
    {
        float timer = 0;
        _isAttacking = true;
        _currentVelocity = Vector3.zero;
        Vector3 startEnemyPosition = enemyTransform.position;

        while (timer < _attackTime)
        {
            Vector3 enemyVector = (startEnemyPosition - t.position);
            if (enemyTransform)
            {
                enemyVector = (enemyTransform.position - t.position);
                //Vector3 attackDirection = Vector3.ProjectOnPlane(enemyVector.normalized, Vector3.up);
            }
            t.Translate(enemyVector * (Time.fixedDeltaTime / _goTime));

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        t.position = new Vector3(t.position.x, 0.5f, t.position.z);

        _currentVelocity *= 0.2f;
        _attackedEnemy = null;
        _isAttacking = false;
    }


    private IEnumerator GoCoroutine(Vector3 direction)
    {
        float timer = 0;
        capsuleCollider.isTrigger = true;
        while (timer < _goTime)
        {
            //float percentage = timer / _goTime;
            t.Translate(direction * (Time.fixedDeltaTime / _goTime));

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (direction.y > 0f)
            t.position = new Vector3(t.position.x, 0.5f, t.position.z);
        else
            t.position = new Vector3(t.position.x, -1f, t.position.z);
        capsuleCollider.isTrigger = false;

    }

    private IEnumerator AttackCooldown()
    {
        _canAttack = false;

        yield return new WaitForSeconds(_attackCooldown);

        _canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && _isAttacking && _canAttack)
        {
            //_attackedEnemy.GetComponent<EnemyHP>().reduceHP(1);
            other.gameObject.GetComponent<EnemyHP>().reduceHP(1);
            AttackCooldown();

        }
    }
}
