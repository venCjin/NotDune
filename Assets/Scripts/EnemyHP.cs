﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int MaxHP = 2;
    public int _hp;
    private Material _material;
    // Start is called before the first frame update
    void Start()
    {
        _hp = MaxHP;
        _material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(_hp  <= 0)
        {
            Destroy(gameObject);
        }
        if(_hp != MaxHP)
        {
            _material.color = Color.red;
        }
        else
        {
            _material.color = Color.white;
        }
        if(_hp > MaxHP)
        {
            _hp = MaxHP;
        }
        
    }

    public int GetHP()
    {
        return _hp;
    }

    public void reduceHP(int dmg)
    {
        _hp -= dmg;
    }
}
