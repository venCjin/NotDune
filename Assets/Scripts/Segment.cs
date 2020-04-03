using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{

    private Rigidbody _rb;
    private Transform _t;
    public Transform _target;
    private CharacterController _characterController;
    private Material _material;
    private Material _playerMaterial;


    void Start()
    {
        //_speed = AboveGroundSpeed;
        _rb = GetComponent<Rigidbody>();
        _t = transform;
        _characterController = FindObjectOfType<CharacterController>();
        _characterController.OnHide += OnCharacterHide;
        _characterController.OnUnhide += OnCharacterUnhide;

        _material = GetComponentInChildren<MeshRenderer>().material;
        _playerMaterial = _characterController.GetComponentInChildren<MeshRenderer>().material;
    }

    private void OnCharacterUnhide()
    {
    }

    private void OnCharacterHide()
    {
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(_target.position, transform.position) > 0.4f)
        {
            transform.position = _target.position + (transform.position - _target.position).normalized * 0.4f;
        }

        transform.LookAt(_target.position);

        _material.color = _playerMaterial.color;
    }

    private void OnDestroy()
    {
        if (_characterController == null) { return; }

        _characterController.OnHide -= OnCharacterHide;
        _characterController.OnUnhide -= OnCharacterUnhide;
    }
}
