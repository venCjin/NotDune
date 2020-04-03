using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Mine : MonoBehaviour
{
    private CharacterController _character;

    [System.Serializable]
    public class Parameters
    {
        public float delay = 0.5f;
        public float blastRadius = 10.0f;
        public float blastDamage = 60.0f;
    }

    [System.Serializable]
    public class References
    {
        public GameObject geometry;
        public CinemachineImpulseSource impulseSource;
        public GameObject explosionEffect;
    }

    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private void Start()
    {
        _character = FindObjectOfType<CharacterController>();

        _character.OnHide += OnCharacterHide;
        _character.OnUnhide += OnCharacterUnhide;
    }

    private void OnDestroy()
    {
        _character.OnHide -= OnCharacterHide;
        _character.OnUnhide -= OnCharacterUnhide;
    }

    private void OnCharacterHide()
    {
        _references.geometry.SetActive(false);
    }

    private void OnCharacterUnhide()
    {
        _references.geometry.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(_character.gameObject))
        {
            _references.geometry.SetActive(true);
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(_parameters.delay);

        Instantiate(_references.explosionEffect, transform.position, transform.rotation);
        _references.impulseSource.GenerateImpulse();

        // Get nearby objects
        Collider[] hits = Physics.OverlapSphere(transform.position, _parameters.blastRadius);

        foreach (Collider hit in hits)
        {
            // Damage
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            float hitProcent = 1 - (distance / _parameters.blastRadius);
            if (hit.tag.Equals("Player"))
            {
                _character.ReceiveDamage((int)(_parameters.blastDamage * hitProcent));
                //Debug.Log("hitProcent: " + hitProcent + ",  Damage: " + (int)(blastDamage * hitProcent));
            }
            else if (hit.tag.Equals("Enemy"))
            {
                hit.gameObject.GetComponent<IntelligentEnemy>().ReceiveDamage((int)(_parameters.blastDamage * hitProcent));
                //Debug.Log("hitProcent: " + hitProcent + ",  Damage: " + (int)(_parameters.blastDamage * hitProcent));
            }
        }

        Destroy(gameObject);
    }
/*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
*/
}
