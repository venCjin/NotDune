using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontRotateUI : MonoBehaviour
{
    private void Update()
    {
        transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
    }
}
