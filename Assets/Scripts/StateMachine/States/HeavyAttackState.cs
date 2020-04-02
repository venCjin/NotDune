using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HeavyAttackState : AbstractState
{

    [System.Serializable]
    public class Parameters
    {
        public float maxDistance = 2.0f;
        public float maxAngle = 45.0f;
        public float duration = 0.4f;
        public float cooldown = 1.0f;
        [Space()]
        public int damage = 50;
    }


    [System.Serializable]
    public class References
    {
        public Cinemachine.CinemachineImpulseSource impulseSource;
    }

    [Space()]
    [SerializeField] private Parameters _parameters;
    [SerializeField] private References _references;

    private CharacterController _character = null;
    private Coroutine _coroutine = null;
    private float _lastAttackTime = -1.0f;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
    }

    public override bool IsStateReady(ref StateMachine stateMachine)
    {
        return (Input.GetMouseButtonDown(1) && Time.time > _lastAttackTime + _parameters.cooldown);
    }

    public override void OnStateEnter(ref StateMachine stateMachine, AbstractState previousState)
    {
        float GetAngle(Vector3 position)
        {
            return Vector3.Angle(position - _character.transform.position, _character.transform.forward);
        }

        Ray ray = new Ray(_character.transform.position, Vector3.one);
        var nearbyEnemiesHits = Physics.SphereCastAll(ray, _parameters.maxDistance, 0.0f, 1 << LayerMask.NameToLayer("Agents"));

        if (nearbyEnemiesHits.Length > 0)
        {
            nearbyEnemiesHits = nearbyEnemiesHits.OrderBy(e => GetAngle(e.transform.position)).ToArray();
        }

        if (nearbyEnemiesHits.Length > 0 && GetAngle(nearbyEnemiesHits.First().transform.position) < _parameters.maxAngle)
        {
            var closestEnemy = nearbyEnemiesHits.First();
            Debug.DrawLine(_character.transform.position, closestEnemy.transform.position, Color.magenta, 1.0f);
            _coroutine = StartCoroutine(Attacking(closestEnemy.transform.GetComponent<IntelligentEnemy>()));
        }
        else
        {
            Debug.DrawRay(_character.transform.position, _character.transform.forward * _parameters.maxDistance, Color.magenta, 1.0f);
            _coroutine = StartCoroutine(Dashing());
        }

    }

    private IEnumerator Attacking(IntelligentEnemy target)
    {
        Vector3 initialPos = _character.rigidbody.position;
        Vector3 targetPos = target.transform.position;
        targetPos.y = initialPos.y;

        float duration = _parameters.duration * Mathf.InverseLerp(0.0f, _parameters.maxDistance, Vector3.Distance(initialPos, targetPos));

        for (float t = 0; t < duration; t += Time.fixedDeltaTime)
        {
            targetPos = target.transform.position;
            targetPos.y = initialPos.y;

            Vector3 finalPos = Vector3.Lerp(initialPos, targetPos, t / duration);
            Vector3 deltaPos = finalPos - _character.rigidbody.position;

            _character.rigidbody.velocity = deltaPos / Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        FindObjectOfType<TailAction>().OnActionPerformed();

        _references.impulseSource.GenerateImpulse();

        _coroutine = null;
    }

    private IEnumerator Dashing()
    {
        Vector3 initialPos = _character.rigidbody.position;
        Vector3 targetPos = initialPos + _character.transform.forward * _parameters.maxDistance;

        for (float t = 0; t < _parameters.duration; t += Time.fixedDeltaTime)
        {
            Vector3 finalPos = Vector3.Lerp(initialPos, targetPos, t / _parameters.duration);
            Vector3 deltaPos = finalPos - _character.rigidbody.position;

            _character.rigidbody.velocity = (targetPos - initialPos) / (_parameters.duration);

            yield return new WaitForFixedUpdate();
        }

        FindObjectOfType<TailAction>().OnActionPerformed();

        _coroutine = null;
    }

    public override bool IsStateFinished()
    {
        return (_coroutine == null);
    }

    public override void OnStateExit()
    {
        _lastAttackTime = Time.time;
    }

    public override void OnStateFixedUpdate(ref StateMachine stateMachine)
    {

    }

    public override void OnStateUpdate(ref StateMachine stateMachine)
    {

    }
}
