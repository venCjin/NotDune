using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAirLabel : MonoBehaviour
{
    [SerializeField] private Text _text = null;
    private CharacterController _character;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
    }

    private void Update()
    {
        _text.text = _character.air.ToString();
    }
}
