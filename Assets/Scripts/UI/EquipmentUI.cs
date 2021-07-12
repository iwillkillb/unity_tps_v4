using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    EquipmentManager equipmentManager;
    EquipmentSlot[] slots;
    public GameObject equipmentUI;  // Parent of slots.

    // Start is called before the first frame update
    void Start()
    {
        equipmentManager = EquipmentManager.instance;
        equipmentManager.onEquipmentChanged += UpdateUI;

        // Size of slots == Count of EquipmentPart.
        slots = equipmentUI.transform.GetComponentsInChildren<EquipmentSlot>();

        // Off Equipment UI
        equipmentUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Equipment UI Torggle
        if (Input.GetButtonDown("Equipment"))
        {
            // UI is Initialized before activating.
            if (equipmentUI.activeSelf == false)
            {
                UpdateUI();
            }

            // UI Activate
            equipmentUI.SetActive(!equipmentUI.activeSelf);

            // Equipment UI on -> Hide Cursor
            if (equipmentUI.activeSelf)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void UpdateUI()
    {
        // Refresh UI
        for (int i = 0; i < equipmentManager.equipments.Length; i++)
        {
            if (equipmentManager.equipments[i] != null)
            {
                slots[i].AddItem(equipmentManager.equipments[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void Remove()
    {
        //equipmentManager.Unequip(0);
        // Button click -> Unequip.
    }
}
