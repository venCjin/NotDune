using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : StateMachine
{
    [System.Serializable]
    public class Parameters
    {
        public float airUsageTime = 5.0f;
        public float airRestoreTime = 3.0f;
    }

    [System.Serializable]
    public class References
    {
        public Transform transform;
        public Rigidbody rigidbody;
        public GameObject states;
        public ColorController colorController;
    }

    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;
    private float _health = 100.0f;
    private float _air = 100.0f;

    public int health
    {
        get => (int)(_health);
        set
        {
            _health = (int)(value);
            OnHealthChanged?.Invoke((int)(_health));
        }
    }
    public bool isAboveGround
    {
        get
        {
            return (_currentState is AboveGroundMovementState);
        }
    }

    public int air
    {
        get => (int)(_air);
        private set => _air = value;
    }

    public bool isGroundBait = false;
    public GameObject tail;

    public new Transform transform { get => _references.transform; }
    public new Rigidbody rigidbody { get => _references.rigidbody; }
    public GameObject states { get => _references.states; }

    public UnityAction OnHide;
    public UnityAction OnUnhide;
    public UnityAction<int> OnHealthChanged;

    private void Awake()
    {
        OnStateChanged += OnCharacterStateChanged;
    }

    private void OnDestroy()
    {
        OnStateChanged -= OnCharacterStateChanged;
    }

    protected override void Update()
    {
        base.Update();

        if (isAboveGround)
        {
            _air += Time.deltaTime * 100.0f / _parameters.airRestoreTime;
        }
        else
        {
            _air -= Time.deltaTime * 100.0f / _parameters.airUsageTime;
        }

        _air = Mathf.Clamp(_air, 0.0f, 100.0f);
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
        health -= damage;

        if (health <= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        _references.colorController.HighLight();
    }
}
