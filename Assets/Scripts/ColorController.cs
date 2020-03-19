using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    [SerializeField] private Color _highlightColor = Color.red;
    [SerializeField] private float _riseSpeed = 25.0f;
    [SerializeField] private float _fallSpeed = 5.0f;

    private Material _material;

    private float _targetHightlightValie = 0.0f;
    private float _currentHighlightValue = 0.0f;
    private Color _idleColor = Color.clear;

    private void Awake()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _idleColor = _material.color;
    }

    private void Update()
    {
        if (_targetHightlightValie > 0.0f || _currentHighlightValue > 0.0f)
        {
            _targetHightlightValie = Mathf.Lerp(_targetHightlightValie, 0.0f, _fallSpeed * Time.deltaTime);
            _currentHighlightValue = Mathf.Lerp(_currentHighlightValue, _targetHightlightValie, _riseSpeed * Time.deltaTime);
            _material.color = Color.Lerp(_idleColor, _highlightColor, _currentHighlightValue);
        }
    }

    public void HighLight()
    {
        _targetHightlightValie = 1.0f;
    }
}
