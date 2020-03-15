using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    // Start is called before the first frame update

    public List<Material> materials = new List<Material>();
    private bool onSurface = true;
    void Start()
    {
        materials.Add(GetComponent<MeshRenderer>().material);
        foreach (var m in GetComponentsInChildren<MeshRenderer>())
        {
            materials.Add(m.material);
        }

        UpdateMaterial();
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {      
            onSurface = !onSurface;
            UpdateMaterial();     
        }
    }

    private void UpdateMaterial()
    {
        foreach (var material in materials)
        {
            if (onSurface)
            {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                    material.SetOverrideTag("RenderType", "Opaque");
                    //material.renderQueue = 2000;
            }
            else 
            {
                    material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
                    material.SetOverrideTag("RenderType", "Transparent");
                    //material.renderQueue = 3000;
            }

            material.SetInt("_OnSurface", (onSurface) ? 1 : 0);
        }
    }
}
