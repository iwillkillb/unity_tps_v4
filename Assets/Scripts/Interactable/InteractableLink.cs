using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableLink : Interactable
{
    public Interactable[] links;

    protected override void StartInteraction()
    {
        base.StartInteraction();

        foreach (Interactable link in links)
        {
            link.OnFocused(interactor);
        }
    }
}
