using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlot : InventorySlot
{
    public EquipmentPart part;

    public new void OnRemoveButton()
    {
        EquipmentManager.instance.Unequip((int)part);
    }
}
