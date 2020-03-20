using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ChangeCompareFunction : MonoBehaviour
{
    private void Awake()
    {
        Image image = GetComponent<Image>();
        Material existingGlobalMat = image.materialForRendering;
        Material updatedMaterial = new Material(existingGlobalMat);
        updatedMaterial.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
        image.material = updatedMaterial;
    }
}