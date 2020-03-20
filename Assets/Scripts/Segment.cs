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
    //public float AboveGroundSpeed;
    //public float UnderGroundSpeed;
    //private float _speed;
    //public float _distanceBeetweenSegments;


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
        //_speed = AboveGroundSpeed;
    }

    private void OnCharacterHide()
    {
        //_speed = UnderGroundSpeed;
    }

    void FixedUpdate()
    {
        //float step = _speed * Vector3.Distance(target.position, transform.position);
        /*if (Vector3.Distance(_target.position, transform.position) > _distanceBeetweenSegments)
        {
            transform.Translate(Vector3.forward * step * Time.fixedDeltaTime);
        }*/

        transform.LookAt(_target.position);
        _rb.velocity = _t.forward * _characterController.rigidbody.velocity.magnitude;
        _material.color = _playerMaterial.color;
    }

    private void OnDestroy()
    {
        _characterController.OnHide -= OnCharacterHide;
        _characterController.OnUnhide -= OnCharacterUnhide;
    }
}
