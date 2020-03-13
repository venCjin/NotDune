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
}
