using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private CharacterController _character;

    public GameObject geometry;

    public GameObject explosionEffect;
    public float delay = 0.5f;
    public float blastRadius = 10.0f;
    public float blastDamage = 60.0f;

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
        geometry.SetActive(false);
    }

    private void OnCharacterUnhide()
    {
        geometry.SetActive(true);
    }






    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(_character.gameObject))
        {
            geometry.SetActive(true);
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(delay);

        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Get nearby objects
        Collider[] hits = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider hit in hits)
        {
            // Damage
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            float hitProcent = 1 - (distance / blastRadius);
            if (hit.tag.Equals("Player"))
            {
                _character.ReceiveDamage((int)(blastDamage * hitProcent));
                //Debug.Log("hitProcent: " + hitProcent + ",  Damage: " + (int)(blastDamage * hitProcent));
            }
            else if (hit.tag.Equals("Enemy"))
            {
                hit.gameObject.GetComponent<IntelligentEnemy>().ReceiveDamage((int)(blastDamage * hitProcent));
                Debug.Log("hitProcent: " + hitProcent + ",  Damage: " + (int)(blastDamage * hitProcent));
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
