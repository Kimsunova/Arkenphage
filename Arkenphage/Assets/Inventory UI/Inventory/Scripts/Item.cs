using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

    new public string name = "New Item";
    public Sprite icon = null;
    public bool showInInventory = true;

    public enum Type {  Inventory = 0,
                        Equipment = 1,
                        Potions = 2,
                        Items = 3 };
    public Type inventoryType;

}
