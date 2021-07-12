using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;

    protected override void StartInteraction()
    {
        base.StartInteraction();

        bool wasPickup = Inventory.instance.Add(item);
        if (wasPickup)
            Destroy(gameObject);
    }
}
