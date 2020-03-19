using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public float AboveGroundSpeed;
    public float UnderGroundSpeed;
    private float _speed;
    public Transform target;
    private CharacterController _characterController;
    public float _distanceBeetweenSegments;
    private Material _material;
    private Material _playerMaterial;




    // Start is called before the first frame update
    void Start()
    {
        _speed = AboveGroundSpeed;
        _characterController = FindObjectOfType<CharacterController>();
        _characterController.OnHide += OnCharacterHide;
        _characterController.OnUnhide += OnCharacterUnhide;

        _material = GetComponentInChildren<MeshRenderer>().material;
        _playerMaterial = _characterController.GetComponentInChildren<MeshRenderer>().material;
    }

    private void OnCharacterUnhide()
    {
        _speed = AboveGroundSpeed;
    }

    private void OnCharacterHide()
    {
        _speed = UnderGroundSpeed;
    }

    void FixedUpdate()
    {
        float step = _speed * Vector3.Distance(target.position, transform.position);
        if (Vector3.Distance(target.position, transform.position) > _distanceBeetweenSegments)
        {
            transform.Translate(Vector3.forward * step * Time.fixedDeltaTime);
        }
        transform.LookAt(target.position);
        _material.color = _playerMaterial.color;
    }

    private void OnDestroy()
    {
        _characterController.OnHide -= OnCharacterHide;
        _characterController.OnUnhide -= OnCharacterUnhide;
    }
}
