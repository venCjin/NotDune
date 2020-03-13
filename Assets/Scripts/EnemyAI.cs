using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private Transform _target;
    private float _shotRange = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        if (_characterController.isAboveGround)
        {
            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance >= _shotRange) // go to player
            {
                _agent.isStopped = false;
                _agent.SetDestination(_target.position);
            }
            else // player is in shot range
            {
                _agent.isStopped = true;
                Shoot();
            }
        }
    }

    void Shoot()
    {

    }
}
