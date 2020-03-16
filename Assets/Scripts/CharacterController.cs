using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{

    public enum State { AboveGround, UnderGround, None };
    private State _state = State.None;
    public UnityAction<State> OnStateChanged = null;

    public Transform t;
    public Rigidbody rb;

    [SerializeField] private float _aboveGroundSpeed = 5f;
    [SerializeField] private float _underGroundSpeed = 12f;

    [SerializeField] private float _currentMaxSpeed = 0f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _deceleration = 10f;
    [SerializeField] private float _attackForce = 20f;

    private Vector2 _currentInput = Vector2.zero;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _currentDirection = Vector3.zero;

    // ATTACK
    [SerializeField] private bool _isAttacking = false;
    [SerializeField] private float _attackTime = 0.35f;
    [SerializeField] private float _attackMaxSpeed = 12f;
    [SerializeField] private float _attackAcceleration = 8f;
    [SerializeField] private float _aboveGroundAttackRadius = 5f;
    [SerializeField] GameObject _attackedEnemy = null;
    private void Start()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
        ChangeState(State.AboveGround);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState(State.AboveGround);
            Attack();
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

        rb.transform.position -= 1.25f * Vector3.up;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;

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

        rb.transform.position += 1.25f * Vector3.up;
        rb.velocity = Vector3.zero;
        rb.useGravity = true;

        _currentMaxSpeed = _aboveGroundSpeed;
        return true;
    }

    private void Attack()
    {
        Vector3 attackDirection = Vector3.zero;


        foreach (EnemyAI enemy in GameManager.instance.enemiesList)
        {
            attackDirection = (enemy.transform.position - t.position).normalized;

            if (Vector3.Dot(attackDirection, t.forward) > 0.5f && attackDirection.magnitude < _aboveGroundAttackRadius/*(_isAboveGround) ? aboveGroundAttackRadius : underGroundAttackRadius*/)
            {
                _attackedEnemy = enemy.gameObject;
                StartCoroutine(AttackCoroutine(attackDirection, enemy.transform));
                break;
            }
        }
        attackDirection = t.forward;
        StartCoroutine(AttackCoroutine(attackDirection, null));

        //rb.AddForce(attackDirection * _attackForce, ForceMode.Impulse);
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

    private void OnTriggerEnter(Collider other)
    {
        if (_isAttacking && other.gameObject == _attackedEnemy)
        {
            _attackedEnemy.GetComponent<EnemyHP>().reduceHP(1);
            Debug.Log("ENEMY");
        }
    }
}
