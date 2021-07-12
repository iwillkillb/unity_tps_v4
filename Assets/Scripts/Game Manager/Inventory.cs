using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This code manages inventory data.

public class Inventory : MonoBehaviour
{
    #region Singletone
    public static Inventory instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found.");
            return;
        }

        instance = this;
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallBack;

    public int inventorySize = 10;

    public Item[] items;    // I need blank in inventory.
    //public List<Item> items = new List<Item>();

    private void Start()
    {
        items = new Item[inventorySize];
    }

    public bool Add(Item item)
    {
        if (IsFull())
        {
            Debug.Log("Not enough inventory.");
            return false;
        }

        // Add Item
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                break;
            }
        }

        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }

        return true;
    }

    public bool Add(Item item, int index)
    {
        if (IsFull())
        {
            Debug.Log("Not enough inventory.");
            return false;
        }

        // Add Item
        if (items[index] == null)
            items[index] = item;

        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }

        return true;
    }

    public bool IsFull()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                return false;
            }
        }

        return true;
    }

    public void Remove(int index)
    {
        // Delete from list
        items[index] = null;

        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }
    }

    public void SwapInInventory(int index0, int index1)
    {
        if (index0 == index1)
            return;

        // Swap index in array
        if (index0 >= 0 && index0 < inventorySize && index1 >= 0 && index1 < inventorySize)
        {
            Item temp = items[index0];
            items[index0] = items[index1];
            items[index1] = temp;
        }

        if (onItemChangedCallBack != null)
        {
            onItemChangedCallBack.Invoke();
        }
    }
}
