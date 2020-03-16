using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private Transform _target;
    private float _shotRange = 5.0f;
    public ParticleSystem rippleParticle;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.enemiesList.Add(this);
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = this.gameObject.GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        _characterController.OnStateChanged += OnStateChanged;


        rippleParticle.Stop();
    }

    private void OnDestroy()
    {
        _characterController.OnStateChanged -= OnStateChanged;
        GameManager.instance.enemiesList.Remove(this);
    }

    void FixedUpdate()
    {

        /*if (_characterController._isAboveGround)
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
        }*/

    }

    private void OnStateChanged(CharacterController.State state)
    {
        if (state == CharacterController.State.AboveGround)
        {
            rippleParticle.Clear();
            rippleParticle.Stop();

            meshRenderer.enabled = true;
        }
        else
        {
            rippleParticle.Play();
            meshRenderer.enabled = false;
        }
    }

    void Shoot()
    {

    }
}
