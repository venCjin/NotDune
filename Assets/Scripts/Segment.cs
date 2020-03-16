﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public float AboveGroundSpeed;
    public float UnderGroundSpeed;
    private float _speed;
    public Transform Target;
    private CharacterController _characterController;

    [SerializeField]
    private float _distanceBeetweenSegments = .1f;



    // Start is called before the first frame update
    void Start()
    {
        _speed = AboveGroundSpeed;
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        _characterController.OnStateChanged += OnStateChange;
        _distanceBeetweenSegments = .7f;
    }

    // Update is called once per frame
    void Update()
    {
        float step = _speed * Time.deltaTime;
        if(Vector3.Distance(Target.position, transform.position) > _distanceBeetweenSegments)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.position, step);
        }
        transform.LookAt(Target.position);
        transform.Rotate(90, 0, 0);

    }

    private void OnStateChange(CharacterController.State state)
    {
        
        if(state == CharacterController.State.AboveGround)
        {
            _speed = AboveGroundSpeed;
           
        }
        else
        {
            _speed = UnderGroundSpeed;
        }
    }

    private void OnDestroy()
    {
        _characterController.OnStateChanged -= OnStateChange;
    }
}
