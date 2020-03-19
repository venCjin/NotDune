using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialManager : MonoBehaviour
{
    public List<Material> materials = new List<Material>();
    private CharacterController _characterController;
    private bool _onSurface;

    void Start()
    {
        foreach (var m in GetComponentsInChildren<Renderer>())
        {
            materials.Add(m.material);
        }
        UpdateMaterial(typeof(AboveGroundMovementState));

        _characterController = FindObjectOfType<CharacterController>();
        _characterController.OnStateChanged += UpdateMaterial;
    }

    // Update is called once per frame
    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && )
    //    {
    //        UpdateMaterial();     
    //    }
    //}

    private void UpdateMaterial(Type state)
    {
        foreach (var material in materials)
        {
            _onSurface = (state == typeof(AboveGroundMovementState));
            //if (state == typeof(AboveGroundMovementState))
            if (_onSurface)
            {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                    material.SetOverrideTag("RenderType", "Opaque");
                    //material.renderQueue = 2000;
            }
            //else if (state == typeof(UnderGroundMovementState))
            else
            {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                    material.SetOverrideTag("RenderType", "Transparent");
                    //material.renderQueue = 3000;
            }

            material.SetInt("_OnSurface", (_onSurface) ? 1 : 0);
        }
    }
}
