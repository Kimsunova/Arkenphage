using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour {

    public Transform itemsParent;
    public GameObject inventoryUI;

    Inventory inventory;

    public InventorySlot[] slots;

    public int pageCount, pageNum = 0; //use these for switching pages in inventory
    public int groupCount, groupNum = 0; //use this for inventory type
    public UnityEngine.UI.Text groupName;
    public List<Item.Type> itemPages = new List<Item.Type>();

    // Use this for initialization
    void Start () {
        inventory = Inventory.instance;
        //inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        groupCount = System.Enum.GetNames(typeof(Item.Type)).Length;
        print(groupCount);

        for (int i = 0; i < groupCount; i++)
        {
            itemPages.Add((Item.Type)i);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            pageNum = 0;
            groupNum = 0;
            UpdateUI(SortItemsByType(pageNum));
        }
    }

    public void InventoryPageLeft()
    {
        if (pageNum > 0)
        {
            pageNum--;
            UpdateUI(SortItemsByType(groupNum));
        }
    }

    public void InventoryPageRight()
    {
        print(pageNum + "  " + pageCount);
        if (pageNum <= pageCount)
        {
            pageNum++;
            UpdateUI(SortItemsByType(groupNum));
        }
    }

    public void GroupPageLeft()
    {
        if (groupNum > 0)
        {
            groupNum--;
            pageNum = 0;

            UpdateUI(SortItemsByType(groupNum));
        }
    }

    public void GroupPageRight()
    {
        if (groupNum < groupCount - 1)
        {
            groupNum++;
            pageNum = 0;

            UpdateUI(SortItemsByType(groupNum));
        }
    }

    public List<Item> SortItemsByType(int group)
    {
        List<Item> newList = new List<Item>();
        Item.Type itemType = itemPages[group];

        groupName.text = itemType.ToString();

        if (group > (int)Item.Type.Inventory)
        {
            foreach (Item item in inventory.items)
            {
                if (item.inventoryType == itemType)
                {
                    newList.Add(item);
                }
            }
        }
        else
        {
            newList.AddRange(inventory.items);
        }

        return newList;
    }

    void UpdateUI(List<Item> newInventory)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int pageChange = (slots.Length * pageNum) + i;
            if (i < slots.Length && pageChange < newInventory.Count)
            {
                slots[i].AddItem(newInventory[pageChange]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
