using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private CharacterController _characterController;

    public bool canAnybodySeePlayer { get; private set; }
    public Vector3 lastKnownChatacterPosition { get; private set; }

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
    }

    private void FixedUpdate()
    {
        canAnybodySeePlayer = CanAnybodySeePlayer();
    }

    private bool CanAnybodySeePlayer()
    {
        foreach (var enemy in FindObjectsOfType<IntelligentEnemy>())
        {
            if (enemy.canSeeCharacter)
            {
                lastKnownChatacterPosition = _characterController.transform.position;
                return true;
            }
        }

        return false;
    }
}
