using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderGroundMovementState : AbstractState
{
    [Space()]
    [SerializeField] private float _maxSpeed = 6f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _deceleration = 10f;

    private CharacterController _characterController;

    private Vector2 _currentInput = Vector2.zero;
    private Vector3 _currentVelocity = Vector3.zero;
    private Vector3 _currentDirection = Vector3.zero;

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
    }

    public override bool IsStateFinished()
    {
        bool buttonPressed = Input.GetKeyDown(KeyCode.Space);
        bool isSurfaceEmpty = (Physics.CheckSphere(transform.position + 1.5f * Vector3.up, 0.5f, 1) == false);

        return (buttonPressed && isSurfaceEmpty);
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        bool buttonPressed = Input.GetKeyDown(KeyCode.Space);

        return buttonPressed;
    }

    public override void OnStateEnter(ref StateMachine stateMachine)
    {
        _characterController.rigidbody.transform.position -= 1.25f * Vector3.up;

        _characterController.rigidbody.velocity = Vector3.zero;
        _characterController.rigidbody.useGravity = false;
    }

    public override void OnStateExit()
    {
        _currentInput = Vector2.zero;
        _currentVelocity = Vector3.zero;
        _currentDirection = Vector3.zero;
    }   

    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
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
            _currentVelocity = Vector3.Lerp(_currentVelocity, _currentDirection * _maxSpeed, _acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, _deceleration * Time.fixedDeltaTime);
        }

        _characterController.rigidbody.velocity = new Vector3(_currentVelocity.x, _characterController.rigidbody.velocity.y, _currentVelocity.z);
    }
}
