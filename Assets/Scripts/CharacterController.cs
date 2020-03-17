using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : StateMachine
{
    [System.Serializable]
    public class References
    {
        public Transform transform;
        public Rigidbody rigidbody;
        public GameObject states;
        public ColorController colorController;
    }

    [SerializeField] private References _references;

    public bool isAboveGround
    {
        get
        {
            return (_currentState is AboveGroundMovementState);
        }
    }

    public new Transform transform { get => _references.transform; }
    public new Rigidbody rigidbody { get => _references.rigidbody; }
    public GameObject states { get => _references.states; }

    public UnityAction OnHide;
    public UnityAction OnUnhide;

    private void Awake()
    {
        OnStateChanged += OnCharacterStateChanged;
    }

    private void OnDestroy()
    {
        OnStateChanged -= OnCharacterStateChanged;
    }

    private void OnCharacterStateChanged(Type type)
    {
        if (type == typeof(AboveGroundMovementState))
        {
            OnUnhide?.Invoke();
        }
        else if (type == typeof(UnderGroundMovementState))
        {
            OnHide?.Invoke();
        }
    }

    public void ReceiveDamage(int damage)
    {
        _references.colorController.HighLight();
    }
}
