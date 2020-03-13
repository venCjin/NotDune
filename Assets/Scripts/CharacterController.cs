using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 6f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _deceleration = 10f;

    private Vector2 _currentInput;

    private float _currentSpeed = 0;
    private Vector3 _currentVelocity;
    private Vector3 _currentDirection;

    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
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

        _rigidBody.velocity = new Vector3(_currentVelocity.x, _rigidBody.velocity.y, _currentVelocity.z);
    }
}
