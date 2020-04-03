using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    private CharacterController _characterController;

    public bool canAnybodySeePlayer { get; private set; }
    public bool hasAnybodyDetectedPlayer { get; private set; }

    public Vector3 lastKnownChatacterPosition { get; private set; }

    public UnityAction OnSomebodySawPlayer;

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
    }

    private void FixedUpdate()
    {
        canAnybodySeePlayer = CanAnybodySeePlayer();
        hasAnybodyDetectedPlayer = HasAnybodyDetectedPlayer();
    }

    private bool HasAnybodyDetectedPlayer()
    {
        foreach (var enemy in FindObjectsOfType<IntelligentEnemy>())
        {
            if (enemy.hasDetectedCharacter)
            {
                lastKnownChatacterPosition = _characterController.transform.position;
                return true;
            }
        }

        return false;
    }

    private bool CanAnybodySeePlayer()
    {
        foreach (var enemy in FindObjectsOfType<IntelligentEnemy>())
        {
            if (enemy.canSeeCharacter)
            {
                lastKnownChatacterPosition = _characterController.transform.position;

                if (canAnybodySeePlayer == false)
                {
                    OnSomebodySawPlayer?.Invoke();
                }

                return true;
            }
        }

        return false;
    }
}
