using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Material material;
    private CharacterController _characterController;

    void Start()
    {
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        _characterController.OnStateChanged += OnStateChanged;

        material = GetComponent<MeshRenderer>().material;
        OnStateChanged(CharacterController.State.AboveGround);
    }   

    private void OnStateChanged(CharacterController.State state)
    {
        if (state == CharacterController.State.AboveGround)
        {
            material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
            material.SetOverrideTag("RenderType", "Opaque");
            //material.renderQueue = 2000;
        }
        else if (state == CharacterController.State.UnderGround)
        {
            material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            material.SetOverrideTag("RenderType", "Transparent");
            //material.renderQueue = 3000;
        }

        material.SetInt("_OnSurface", (state == CharacterController.State.AboveGround) ? 1 : 0);
    }
}
