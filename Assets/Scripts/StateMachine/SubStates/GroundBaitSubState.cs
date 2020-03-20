﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBaitSubState : AbstractSubState
{
    private CharacterController _character;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        bool buttonPressed = Input.GetMouseButtonDown(1);

        return buttonPressed;
    }
    
    public override void OnStateEnter(ref StateMachine stateMachine)
    {
        _character.rigidbody.transform.position += new Vector3(0.0f, 0.5f, 0.0f);
        _character.isGroundBait = true;
    }
    
    public override void OnStateUpdate(ref StateMachine stateMachine)
    {
        
    }
    
    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
    {

    }
    
    public override bool IsStateFinished()
    {
        bool buttonPressed = Input.GetMouseButtonUp(1);

        return buttonPressed;
    }
    
    public override void OnStateExit()
    {
        _character.rigidbody.transform.position -= new Vector3(0.0f, 0.5f, 0.0f);
        _character.isGroundBait = false;
    }
}