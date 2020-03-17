using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundbaitAction : AbstractAction
{
    private CharacterController _character;

    private void Awake()
    {
        _character = FindObjectOfType<CharacterController>();
    }

    public override bool IsActionReady()
    {
        return (Input.GetMouseButtonDown(1) == true);
    }

    public override void OnActionPerformed()
    {
        // wystaw ogon

        // jesli ogon colliduje z widzeniem enemy => trigger enemy

    }

}
