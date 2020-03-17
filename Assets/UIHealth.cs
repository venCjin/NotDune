using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private Text _text = null;
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
        _text.text = health.ToString();
    }
}
