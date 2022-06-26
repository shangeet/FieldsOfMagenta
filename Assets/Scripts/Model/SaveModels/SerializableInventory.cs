using System.Collections;
using System.Collections.Generic;

public class SerializableInventory {

    public List<SerializableItem> listOfItems;


    public SerializableInventory() {}


    public SerializableInventory(List<ConsumableItem> cItemList, List<EquipmentItem> eItemList) {
        this.listOfItems = new List<SerializableItem>();
        foreach(ConsumableItem cItem in cItemList) {
            this.listOfItems.Add(SerializableItem.ConvertFromConsumableItem(cItem));
        }
        foreach(EquipmentItem eItem in eItemList) {
            this.listOfItems.Add(SerializableItem.ConvertFromEquipmentItem(eItem));
        }
    }

    public static SerializableInventory ConvertFromInventory(Inventory inventory) {
        return new SerializableInventory(inventory.consumableItems, inventory.equipmentItems);
    }
    
    public static Inventory ConvertToInventory(SerializableInventory inventory) {
        Inventory newInventory = new Inventory();
        foreach(SerializableItem sItem in inventory.listOfItems) {
            if (sItem.itemType == ItemType.CONSUMABLE) {
                newInventory.consumableItems.Add(SerializableItem.ConvertToConsumableItem(sItem));
            } else if (sItem.itemType == ItemType.EQUIPMENT) {
                newInventory.equipmentItems.Add(SerializableItem.ConvertToEquipmentItem(sItem));
            }
        }
        return newInventory;
    }
}