using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyhealth : MonoBehaviour
{
    [SerializeField] private Slider _slider = null;
    private IntelligentEnemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<IntelligentEnemy>();

        _enemy.OnHealthChanged += OnCharacterHealthChanged;

        _slider.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _enemy.OnHealthChanged -= OnCharacterHealthChanged;
    }

    private void OnCharacterHealthChanged(int health)
    {
        _slider.gameObject.SetActive(true);
        _slider.value = (1.0f * health) / (100.0f);
    }
}
