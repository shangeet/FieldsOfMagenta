using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemManager {

    public Dictionary<string,int> currentConsumableItemsInventory = new Dictionary<string, int>() {};
    public Dictionary<string, ConsumableItem> consumableItems = new Dictionary<string, ConsumableItem>();
    public static int numMaxSlots = 6;
    public static int numMaxItemsPerSlot = 3;

    public void PopulateWithCurrentitems(List<ConsumableItem> itemList) {
        foreach(ConsumableItem item in itemList) {
            consumableItems[item.itemName] = item;
            if (currentConsumableItemsInventory.ContainsKey(item.itemName)) {
                currentConsumableItemsInventory[item.itemName]++;
            } else {
                currentConsumableItemsInventory[item.itemName] = 1;
            }
        }
    }

    public void AddItem(ConsumableItem newItem) {
    
        int currentUsedSlots = currentConsumableItemsInventory.Keys.Count;
        int currentItemsInSlot = currentConsumableItemsInventory.ContainsKey(newItem.itemName) ? currentConsumableItemsInventory[newItem.itemName] : 0;
    
        if ((currentConsumableItemsInventory.ContainsKey(newItem.itemName) && currentItemsInSlot < numMaxItemsPerSlot) ||
            (!currentConsumableItemsInventory.ContainsKey(newItem.itemName) && currentUsedSlots <= numMaxSlots)) {
            consumableItems[newItem.itemName] = newItem;
            currentConsumableItemsInventory[newItem.itemName] = currentConsumableItemsInventory.ContainsKey(newItem.itemName) ? currentConsumableItemsInventory[newItem.itemName]++ : 1;
        }    
    }

    public void ConsumeItem(ConsumableItem itemToConsume) {

        if (currentConsumableItemsInventory.ContainsKey(itemToConsume.itemName)) {
            int currentItemsInSlot = currentConsumableItemsInventory[itemToConsume.itemName];
            if (currentItemsInSlot == 1) {
                currentConsumableItemsInventory.Remove(itemToConsume.itemName);
                consumableItems.Remove(itemToConsume.itemName);
            } else {
                currentConsumableItemsInventory[itemToConsume.itemName]--;
            }
        }
    }
}
