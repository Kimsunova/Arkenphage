using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static Inventory instance;

    public List<Item> items = new List<Item>();

    public int inventorySpace = 0;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Inventory instance");
            return;
        }
        else
        {
            instance = this;
        }
    }

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    // Add a new item if enough room
    public void AddToInventory(Item item)
    {
        //if (item.showInInventory)
        //{
            /*if (items.Count >= inventorySpace)
            {
                Debug.Log("Not enough room.");
                return;
            }*/

            items.Add(item);

            //if (onItemChangedCallback != null)
            //    onItemChangedCallback.Invoke();
        //}
    }

    // Remove an item
    public void RemoveFromInventory(Item item)
    {
        items.Remove(item);

        //if (onItemChangedCallback != null)
        //    onItemChangedCallback.Invoke();
    }
}
