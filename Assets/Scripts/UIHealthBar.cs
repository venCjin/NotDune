using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Image _image = null;
    private CharacterController _character;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();

        _character.OnHealthChanged += OnCharacterHealthChanged;
    }

    private void OnDestroy()
    {
        _character.OnHealthChanged -= OnCharacterHealthChanged;
    }

    private void OnCharacterHealthChanged(int health)
    {
        _image.fillAmount = 0.5f * (1.0f * health) / (100.0f);
    }
}
