using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{

    public enum State { AboveGround, UnderGround, None };
    private State _state = State.None;
    public UnityAction<State> OnStateChanged = null;

    [System.Serializable]
    public class References
    {
        public Transform transform;
        public Rigidbody rigidbody;
        public GameObject states;
    }

    [SerializeField] private References _references;

    public bool _isAboveGround = true;
  

    public new Transform    transform  { get => _references.transform; }
    public new Rigidbody    rigidbody  { get => _references.rigidbody; }
    public GameObject       states     { get => _references.states; }

    [SerializeField] private float _aboveGroundSpeed = 3f;
    [SerializeField] private float _underGroundSpeed = 6f;

    [SerializeField] private float _currentMaxSpeed = 0f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _deceleration = 10f;


    private Vector2 _currentInput = Vector2.zero;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _currentDirection = Vector3.zero;

    private void Start()
    {
        ChangeState(State.AboveGround);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeState(State.AboveGround);
            //Attack();
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
        if (_currentInput.x != 0 || _currentInput.y != 0)
        {
            _currentVelocity = Vector3.Lerp(_currentVelocity, _currentDirection * _currentMaxSpeed, _acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, _deceleration * Time.fixedDeltaTime);
        }

        rigidbody.velocity = new Vector3(_currentVelocity.x, rigidbody.velocity.y, _currentVelocity.z);

    }


    private void ChangeState(State state)
    {
        if (_state == state)
            return;
        else
        {
            _state = state;
            if (state == State.UnderGround)
                GoUnderGround();
            else
                GoAboveGround();

            OnStateChanged?.Invoke(state);
        }


    }

    private void GoUnderGround()
    {
        if (!_isAboveGround) return;

        _isAboveGround = false;

        rigidbody.transform.position -= 1.25f * Vector3.up;
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;

        _currentMaxSpeed = _underGroundSpeed;
    }

    private void GoAboveGround()
    {
        if (_isAboveGround) return;

        _isAboveGround = true;

        rigidbody.transform.position += 1.25f * Vector3.up;
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = true;

        _currentMaxSpeed = _aboveGroundSpeed;
    }

    /*private void Attack()
    {
        Vector3 attackDirection = transform.forward;

        foreach (EnemyAI enemy in GameManager.instance.EnemyAIList)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < (_isAboveGround) ? aboveGroundAttackRadius : underGroundAttackRadius)
            {
                //attackDirection = enemy
            }
        }

        //AddForce attackDirection
    }*/
}
