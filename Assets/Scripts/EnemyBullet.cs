using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public Vector3 dir;
    public float lifetime;
    private float _currnentTime;
    private Rigidbody rb;
    private HP _hp = null;
    // Start is called before the first frame update
    void Start()
    {
        _currnentTime = 0.0f;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = dir * speed;
        _currnentTime += Time.deltaTime;
        if(_currnentTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_hp = other.gameObject.GetComponent<HP>())
        {
            if (other.gameObject.GetComponent<EnemyAI>() == null)
            {
                _hp.reduceHP(1);
                Destroy(gameObject);
            }
            
        }
    }
}
