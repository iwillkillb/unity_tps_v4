using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// This code takes the player's input and displays the inventory window.

public class InventoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Singletone
    public static InventoryUI instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryUI found.");
            return;
        }

        instance = this;
    }
    #endregion

    Inventory inventory;
    InventorySlot[] slots;
    public GameObject inventoryUI;  // Parent of slots.
    public GameObject inventorySlotPrefab;

    // Drag fields
    public Image draggingItemImage;             // Uncheck Raycast Target
    [HideInInspector]public int draggingIndex;  // Get slot's index whenever
    int beginDragIndex;                         // Get slot's index when begin drag
    Color draggingItemImageColor;               // Backup alpha of dragging item's slot image
    bool onPointerOver = false;                 // Check if the cursor is over the InventoryUI.



    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallBack += UpdateUI;

        for(int i=0; i<inventory.inventorySize; i++)
        {
            Instantiate(inventorySlotPrefab, inventoryUI.transform);
        }

        slots = inventoryUI.transform.GetComponentsInChildren<InventorySlot>();

        // Inventory UI Initialization
        UpdateUI();

        // Off Inventory
        inventoryUI.SetActive(false);
    }



    void Update()
    {
        // Inventory UI Torggle
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);

            // Inventory UI on -> Hide Cursor
            if (inventoryUI.activeSelf)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }



    void UpdateUI()
    {
        for (int i = 0; i < inventory.items.Length; i++)
        {
            if (inventory.items[i] != null)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }



    // Events by interface...................................



    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerOver = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Blank slot -> Exit
        if (inventory.items[draggingIndex] == null)
            return;

        // On draggingItemImage Object
        draggingItemImage.gameObject.SetActive(true);
        draggingItemImage.sprite = slots[draggingIndex].icon.sprite;

        // Getting Dragging item's index
        beginDragIndex = draggingIndex;

        // Hide beginDragIndex's icon
        draggingItemImageColor = slots[beginDragIndex].icon.color;
        Color zeroAlphaColor = draggingItemImageColor;
        zeroAlphaColor.a = 0f;
        slots[beginDragIndex].icon.color = zeroAlphaColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move dragging image by cursor
        draggingItemImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Off draggingItemImage Object
        draggingItemImage.gameObject.SetActive(false);
        draggingItemImage.sprite = null;

        // Return beginDragIndex's icon
        slots[beginDragIndex].icon.color = draggingItemImageColor;

        // Items swap
        if (onPointerOver)
            inventory.SwapInInventory(beginDragIndex, draggingIndex);
    }
}
