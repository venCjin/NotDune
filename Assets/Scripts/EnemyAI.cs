using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Attack, Retreat, Idle}
    [SerializeField]
    private EnemyState _enemyState;
    public float fleeRange;
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private Transform _target;
    private float _shotRange = 5.0f;
    public ParticleSystem rippleParticle;
    private MeshRenderer meshRenderer;
    private EnemyHP _HP;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.enemiesList.Add(this);
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = this.gameObject.GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        _characterController.OnStateChanged += OnStateChanged;
        _HP = GetComponent<EnemyHP>();


        rippleParticle.Stop();
    }

    private void OnDestroy()
    {
        _characterController.OnStateChanged -= OnStateChanged;
        GameManager.instance.enemiesList.Remove(this);
    }

    void FixedUpdate()
    {
        if(_enemyState == EnemyState.Attack)
        {
            if (_characterController._isAboveGround)
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
        else if(_enemyState == EnemyState.Retreat)
        {
            _agent.isStopped = false;
            Vector3 v =  transform.position -_target.position;
            v.Normalize();
            v *= fleeRange;
            v += transform.position;

            _agent.SetDestination(v);

            
        }
        else if (_enemyState == EnemyState.Idle)
        {

        }

        if (_HP.GetHP() < _HP.MaxHP)
        {
            //_enemyState = EnemyState.Retreat;
        }

    }

    private void OnStateChanged(CharacterController.State state)
    {
        if (state == CharacterController.State.AboveGround)
        {
            rippleParticle.Clear();
            rippleParticle.Stop();
            if(_HP.GetHP() < _HP.MaxHP)
            {
                _enemyState = EnemyState.Retreat;
            }
            else
            {
                _enemyState = EnemyState.Attack;
            }
            meshRenderer.enabled = true;
        }
        else
        {
            rippleParticle.Play();
            _enemyState = EnemyState.Idle;
            meshRenderer.enabled = false;
        }
        Debug.Log(_enemyState);
    }

    void Shoot()
    {

    }
}
