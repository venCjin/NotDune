using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAirBar : MonoBehaviour
{
    [SerializeField] private Image _image = null;
    private CharacterController _character;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
    }

    private void Update()
    {
        _image.fillAmount = 0.5f * (1.0f * _character.air) / (100.0f);
    }
}
