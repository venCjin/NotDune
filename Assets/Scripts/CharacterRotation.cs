using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    public enum LookDirection { Camera, Movement}

    [SerializeField] LookDirection _lookDirection = LookDirection.Movement;
    private CharacterController _character;

    private void Awake()
    {
        _character = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (_lookDirection == LookDirection.Camera)
        {
            var forward = Camera.main.transform.forward;
            forward = Vector3.ProjectOnPlane(forward, Vector3.up);
            forward.Normalize();

            _character.transform.rotation = Quaternion.LookRotation(forward);
        }
        else if (_lookDirection == LookDirection.Movement)
        {
            var forward = _character.rigidbody.velocity;

            if (forward.magnitude> 0.1f)
            {
                forward = Vector3.ProjectOnPlane(forward, Vector3.up);
                forward.Normalize();

                _character.transform.rotation = Quaternion.LookRotation(forward);
            }
        }
    }
}
