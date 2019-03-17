using UnityEngine;

public class PickupItem : Interactable
{
    public Item item;

    public override void Interact()
    {
        base.Interact();

        PickUp();
    }

    void PickUp()
    {
        print("Pick up " + item.name);
        //if (Inventory.instance.items.Count)
        Inventory.instance.AddToInventory(item);

        Destroy(gameObject);
    }
}

