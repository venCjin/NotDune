using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyDetection : MonoBehaviour
{
    [SerializeField] private Image _image = null;
    [SerializeField] private Image _background = null;
    private IntelligentEnemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<IntelligentEnemy>();
    }

    private void Update()
    {
        if (_enemy.IsCurrentlyInState(typeof(EnemyIdleState)) || _enemy.IsCurrentlyInState(typeof(EnemyPatrolState)))
        {
            _image.color = (_enemy.detectionLevel > 0.0f) ? (Color.green) : (Color.clear);
            _background.color = (_enemy.detectionLevel > 0.0f) ? (Color.white) : (Color.clear);

            _image.fillAmount = _enemy.detectionLevel;
        }
        else if (_enemy.IsCurrentlyInState(typeof(EnemySearchState)))
        {
            _image.color = Color.yellow;
            _background.color = Color.white;

            _image.fillAmount = 1.0f;
        }
        else if (_enemy.IsCurrentlyInState(typeof(EnemyAttackState)))
        {
            _image.color = Color.red;
            _background.color = Color.white;

            _image.fillAmount = 1.0f;
        }

        Vector3 target = Camera.main.transform.position + Vector3.ProjectOnPlane(transform.position - Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 forward = transform.position - target; forward.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(forward);
    }
}
