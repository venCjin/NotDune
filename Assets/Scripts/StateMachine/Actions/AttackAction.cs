using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AbstractAction
{
    [System.Serializable]
    public class Parameters
    {
        [Range(0, 100)] public int damage = 30;
        public float hitDistance = 1.0f;
        public LayerMask layerMask = 1;
        public float hitSphereRaduis = 1.0f;
    }

    [System.Serializable]
    public class References
    {
        public Cinemachine.CinemachineImpulseSource impulseSource;
    }

    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private CharacterController _character;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
    }

    public override bool IsActionReady()
    {
        return (Input.GetMouseButtonDown(0) == true);
    }

    public override void OnActionPerformed()
    {
        Ray ray = new Ray(_character.transform.position + new Vector3(0,0.5f,0), _character.transform.forward);
        var hits = Physics.SphereCastAll(ray, _parameters.hitSphereRaduis, _parameters.hitDistance, _parameters.layerMask.value, QueryTriggerInteraction.Ignore);

        foreach(var hit in hits)
        {
            IDamageable enemy = hit.transform.GetComponent<IDamageable>();

            if (enemy != null)
            {
                enemy.ReceiveDamage(_parameters.damage);
                Debug.Log("Player make " + _parameters.damage + " damege to : " + hit.transform.name);
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
        Gizmos.DrawWireSphere(_character.transform.position + new Vector3(0, 0.5f, 0) + _character.transform.forward * _parameters.hitDistance, _parameters.hitSphereRaduis);
    }
}
