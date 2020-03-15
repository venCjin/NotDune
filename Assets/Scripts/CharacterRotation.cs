using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    private CharacterController _character;

    private void Awake()
    {
        _character = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        var forward = Camera.main.transform.forward;
        forward = Vector3.ProjectOnPlane(forward, Vector3.up);
        forward.Normalize();

        _character.transform.rotation = Quaternion.LookRotation(forward);
    }
}
