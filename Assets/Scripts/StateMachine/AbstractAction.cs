using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAction : MonoBehaviour
{
    public abstract bool IsActionReady();
    public abstract void OnActionPerformed();
}
