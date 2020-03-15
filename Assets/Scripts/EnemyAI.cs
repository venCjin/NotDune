using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private Transform _target;
    private ColorController _colorController;
    private float _shotRange = 5.0f;
    public ParticleSystem rippleParticle;
    private bool wasCharacterAboveGround;
    private MeshRenderer meshRenderer;
    [SerializeField] private ParticleSystem _shootParticle;
    private CinemachineImpulseSource _impulseSource;

    private int _health = 100;

    private float _lastShootTime = 0.0f;
    private float _shootCooldown = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = this.gameObject.GetComponent<NavMeshAgent>();
        _colorController = GetComponent<ColorController>();
        meshRenderer = GetComponent<MeshRenderer>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();

        rippleParticle.Stop();
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

                if (Time.time > _lastShootTime + _shootCooldown)
                Shoot();
            }
        }

        if (_characterController.isAboveGround != wasCharacterAboveGround)
        {
            if (_characterController.isAboveGround)
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

        wasCharacterAboveGround = _characterController.isAboveGround;

    }

    void Shoot()
    {
        FindObjectOfType<CharacterController>().ReceiveDamage(5);
        _impulseSource.GenerateImpulse();

        _shootParticle.Play();
        _lastShootTime = Time.time;
    }

    public void ReceiveDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _colorController.HighLight();
        }
    }
}
