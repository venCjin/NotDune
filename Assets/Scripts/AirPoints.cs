using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirPoints : MonoBehaviour
{
    public float AirTime;
    public float dmgCooldown;
    public GameObject ui;
    public float _air { get; private set; }
    private HP _hp;
    private CharacterController _characterController;
    private float _currentTime;
    private Slider _airBar;

    // Start is called before the first frame update
    void Start()
    {
        _currentTime = 0.0f;
        _hp = GetComponent<HP>();
        _air = AirTime;
        _characterController = GetComponent<CharacterController>();
        _characterController.OnStateChanged += OnStateChanged;
        ui.SetActive(false);
        _airBar = ui.GetComponent<Slider>();
    }

    private void OnStateChanged(CharacterController.State arg0)
    {
        if (arg0 == CharacterController.State.AboveGround)
        {
            _air = AirTime;
            ui.SetActive(false);
        }
        else
        {
            ui.SetActive(true);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(_characterController._state == CharacterController.State.UnderGround)
        {

            _air -= Time.fixedDeltaTime;
            _currentTime += Time.fixedDeltaTime;
            _airBar.value = _air / AirTime;
            if(_air < 0)
            {
                _air = 0;
                if(_currentTime > dmgCooldown)
                {
                    _hp.reduceHP(1);
                    _currentTime = 0;
                }
            }
        }
        else
        {
            if (_air < AirTime)
            {
                _air += Time.fixedDeltaTime;
            }
        }
    }
}
