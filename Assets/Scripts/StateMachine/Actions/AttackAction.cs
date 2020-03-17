using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AbstractAction
{
    public Animator pincerL;
    public Animator pincerR;

    [System.Serializable]
    public class Parameters
    {
        [Range(0, 100)] public int damage = 30;
        public float hitDistance = 1.5f;
        public LayerMask layerMask = 1;
    }

    [System.Serializable]
    public class References
    {
        public Cinemachine.CinemachineImpulseSource impulseSource;
    }

    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private CharacterController _character;
    private float _lastAttackTime = 0.0f;
    private float _attackCooldown = 0.3f;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
    }

    public override bool IsActionReady()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            if (Time.time > _lastAttackTime + _attackCooldown)
            {
                _lastAttackTime = Time.time;
                return true;
            }
        }
        return false;
    }

    public override void OnActionPerformed()
    {
        pincerL.SetTrigger("pincer");
        pincerR.SetTrigger("pincer");

        Ray ray = new Ray(_character.transform.position, _character.transform.forward);
        var hits = Physics.SphereCastAll(ray, 1.5f, _parameters.hitDistance, _parameters.layerMask.value, QueryTriggerInteraction.Ignore);

        foreach (var hit in hits)
        {
            IDamageable enemy = hit.transform.GetComponent<IDamageable>();

            if (enemy != null)
            {
                enemy.ReceiveDamage(_parameters.damage);
                Debug.Log(hit.transform.name + " hit!");
            }
        }

        if (hits.Length > 0)
        {
            _references.impulseSource.GenerateImpulse();
        }
    }

    private void OnDrawGizmos()
    {
        if (_character == null) { return; }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_character.transform.position + _character.transform.forward * _parameters.hitDistance, 1.5f);
    }
}
