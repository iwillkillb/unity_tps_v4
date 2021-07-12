using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Moveable : Interactable
{
    new Rigidbody rigidbody;
    Transform parent;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    protected override void StartInteraction()
    {
        base.StartInteraction();

        // Backup Parent
        parent = transform.parent;

        // Connect with Interactor
        rigidbody.isKinematic = true;
        transform.parent = interactor;
    }

    protected override void EndInteraction()
    {
        base.EndInteraction();

        // Disconnect from Interactor
        rigidbody.isKinematic = false;
        transform.parent = parent;
    }
}
