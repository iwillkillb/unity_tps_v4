using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singletone
    public static EquipmentManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Equipment Manager found.");
            return;
        }

        instance = this;
    }
    #endregion

    public delegate void OnEquipmentChanged();
    public OnEquipmentChanged onEquipmentChanged;

    public delegate void OnEquipmentModifiedChanged(Equipment item, bool isAdding);
    public OnEquipmentModifiedChanged onEquipmentModifiedChanged;

    //public SkinnedMeshRenderer targetMesh;  // Equipments user(Player)'s modeling mesh.

    public Equipment[] equipments;   // Items i currently have equipped.
    //SkinnedMeshRenderer[] currentMeshes;



    // Start is called before the first frame update
    void Start()
    {
        // Initialize equipments based on number of equipment slots.
        int numSlots = System.Enum.GetNames(typeof(EquipmentPart)).Length;
        equipments = new Equipment[numSlots];
        //currentMeshes = new SkinnedMeshRenderer[numSlots];
    }



    // Used by Equipment.cs in Equipment item.
    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;

        Equipment oldItem = Unequip(slotIndex);

        // Insert the item into the slot.
        equipments[slotIndex] = newItem;

        /*
        // Setting mesh
        if (newItem.mesh != null)
        {
            SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(newItem.mesh);
            newMesh.transform.parent = targetMesh.transform;
            newMesh.bones = targetMesh.bones;
            newMesh.rootBone = targetMesh.rootBone;
            currentMeshes[slotIndex] = newMesh;
        }*/

        // Callback
        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke();
        if (onEquipmentModifiedChanged != null)
            onEquipmentModifiedChanged.Invoke(newItem, true);
    }



    public Equipment Unequip(int slotIndex)
    {
        // Is there empty slot in inventory?
        if (Inventory.instance.IsFull())
            return null;

        if (equipments[slotIndex] != null)
        {
            /*
            // Mesh
            if (currentMeshes[slotIndex] != null)
            {
                Destroy(currentMeshes[slotIndex].gameObject);
            }*/

            // Save current equipment for callback.
            Equipment oldItem = equipments[slotIndex];
            Inventory.instance.Add(oldItem);

            // Make empty slot.
            equipments[slotIndex] = null;

            // Callback
            if (onEquipmentChanged != null)
                onEquipmentChanged.Invoke();
            if (onEquipmentModifiedChanged != null)
                onEquipmentModifiedChanged.Invoke(oldItem, false);

            return oldItem;
        }
        return null;
    }

    private int CountCurrentEquipment()
    {
        int countOfEquipments = 0;

        for (int i = 0; i < equipments.Length; i++)
        {
            if (equipments[i] != null)
            {
                countOfEquipments++;
            }
        }

        return countOfEquipments;
    }
}
