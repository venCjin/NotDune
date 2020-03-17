using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLookAtPlayerAction : AbstractAction
{
    CharacterController _character = null;
    IntelligentEnemy _enemy = null;

    private bool _characterTookDamage = false;

    private void Awake()
    {
        _character = GetComponent<CharacterController>();
        _enemy = GetComponentInParent<IntelligentEnemy>();

        _enemy.OnEnemyTookDamage += OnEnemyTookDamage;
    }

    private void OnDestroy()
    {
        _enemy.OnEnemyTookDamage -= OnEnemyTookDamage;
    }

    private void OnEnemyTookDamage()
    {
        _characterTookDamage = true;
    }

    public override bool IsActionReady()
    {
        if (_characterTookDamage && (_enemy.canSeeCharacter == false))
        {
            _characterTookDamage = false;
            return true;
        }
        else
        {
            _characterTookDamage = false;
            return false;
        }
    }

    public override void OnActionPerformed()
    {
        Vector3 forward = _character.transform.position - _enemy.transform.position;
        _enemy.transform.rotation = Quaternion.LookRotation(forward);
    }
}