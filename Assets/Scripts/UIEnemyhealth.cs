using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyhealth : MonoBehaviour
{
    [SerializeField] private Slider _slider = null;
    private CharacterController _character;
    private IntelligentEnemy _enemy;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
        _enemy = GetComponentInParent<IntelligentEnemy>();

        _enemy.OnHealthChanged += OnCharacterHealthChanged;

        _slider.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _enemy.OnHealthChanged -= OnCharacterHealthChanged;
    }

    private void FixedUpdate()
    {
        Vector3 target = Camera.main.transform.position + Vector3.ProjectOnPlane(transform.position - Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 forward = transform.position - target; forward.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(forward);
    }

    private void OnCharacterHealthChanged(int health)
    {
        _slider.gameObject.SetActive(true);
        _slider.value = (1.0f * health) / (100.0f);
    }
}
