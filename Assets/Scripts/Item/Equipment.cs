using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentPart equipSlot;
    public SkinnedMeshRenderer mesh;
    public int attackModifier;
    public int defenseModifier;

    public override void Use(int inventoryIndex)
    {
        base.Use(inventoryIndex);

        // Remove from inventory.
        Inventory.instance.Remove(inventoryIndex);

        // Equip the item
        EquipmentManager.instance.Equip(this);
    }
}

public enum EquipmentPart {Weapon, Armor, Booster, Detector}
